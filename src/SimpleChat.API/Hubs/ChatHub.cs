using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using SimpleChat.Core.Business_Interface;
using SimpleChat.Core.Business_Interface.ServiceQuery;
using SimpleChat.Core.Entities;
using SimpleChat.Core.Model;
using SimpleChat.DataRepositories.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;

namespace SimpleChat.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMessageServiceQuery messageServiceQuery;
        private readonly IMessageService messageService;
        private readonly IConversationServiceQuery conversationServiceQuery;
        private readonly IConfiguration _config;

        public ChatHub(IMessageServiceQuery messageServiceQuery, IMessageService messageService, IConversationServiceQuery conversationServiceQuery, IConfiguration config)
        {
            this.messageServiceQuery = messageServiceQuery;
            this.messageService = messageService;
            this.conversationServiceQuery = conversationServiceQuery;
            _config = config;

        }
        static IList<UserConnection> Users = new List<UserConnection>();

        public class UserConnection
        {
            public string UserId { get; set; }
            public string ConnectionId { get; set; }
            public string FullName { get; set; }
            public string Username { get; set; }
        }

        public void SendMessageToUser(Message message)
        {
            var reciever = Users.FirstOrDefault(x => x.UserId == message.Receiver);
            var connectionId = reciever == null ? "offlineUser" : reciever.ConnectionId;
            message.MessageDate = message.MessageDate.ToLocalTime();
            this.messageService.Add(message);
            conversationServiceQuery.Update(message.ChatId);

            var botToken = _config.GetValue<string>("BotConfiguration:BotToken");
            var api = new BotClient(botToken);
            api.SendMessage(message.ChatId, message.Content); // Send a message to user
        }

        public async Task DeleteMessage(MessageDeleteModel message)
        {
            var deletedMessage = await this.messageService.DeleteMessage(message);
            await Clients.All.SendAsync("BroadCastDeleteMessage", Context.ConnectionId, deletedMessage);
        }

        public async Task PublishUserOnConnect(string id, string fullname, string username)
        {

            var existingUser = Users.FirstOrDefault(x => x.Username == username);
            var indexExistingUser = Users.IndexOf(existingUser);

            UserConnection user = new UserConnection
            {
                UserId = id,
                ConnectionId = Context.ConnectionId,
                FullName = fullname,
                Username = username
            };

            if (!Users.Contains(existingUser))
            {
                Users.Add(user);

            }
            else
            {
                Users[indexExistingUser] = user;
            }

            await Clients.All.SendAsync("BroadcastUserOnConnect", Users);

        }

        public void RemoveOnlineUser(string userID)
        {
            var user = Users.Where(x => x.UserId == userID).ToList();
            foreach (UserConnection i in user)
                Users.Remove(i);

            Clients.All.SendAsync("BroadcastUserOnDisconnect", Users);
        }

        public void MarkAllUnreadMessage(long chatId)
        {
            messageService.MarkAllMessagesAsRead(chatId);
        }

        public void SendMessage(Telegram.Bot.Types.Message message)
        {
            var conversations = conversationServiceQuery.GetAllConversation().Where(x => x.ChatId == message.Chat.Id);
            if (conversations.Any())
            {
                conversationServiceQuery.Update(message.Chat.Id);
            }
            else
            {
                Conversation conversation = new Conversation
                {
                    ChatId = message.Chat.Id,
                    UserName = message.From.Username,
                    FirstName = message.From.FirstName,
                    LastName = message.From.LastName,
                    LastMessageDate = DateTime.Now,
                };
                conversationServiceQuery.Add(conversation);
            }

            Message msg = new Message
            {
                ChatId = message.Chat.Id,
                MessageDate = DateTime.Now,
                Content = message.Text,
                Receiver = Users.FirstOrDefault().UserId
            };
            messageService.Add(msg);

            var reciever = Users.FirstOrDefault(x => x.UserId == msg.Receiver);
            var connectionId = reciever == null ? "offlineUser" : reciever.ConnectionId;
            Clients.Client(connectionId).SendAsync("ReceiveDM", connectionId, msg);
            Clients.All.SendAsync("NewMessage", message.Chat.Id);
        }
    }
}
