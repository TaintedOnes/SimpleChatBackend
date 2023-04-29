using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SimpleChat.Core.Business_Interface;
using SimpleChat.Core.Business_Interface.ServiceQuery;
using SimpleChat.Core.Entities;
using SimpleChat.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
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
        private readonly string _botToken;

        public ChatHub(IMessageServiceQuery messageServiceQuery, IMessageService messageService, IConversationServiceQuery conversationServiceQuery, IConfiguration config)
        {
            this.messageServiceQuery = messageServiceQuery;
            this.messageService = messageService;
            this.conversationServiceQuery = conversationServiceQuery;
            _config = config;
            _botToken = config.GetValue<string>("BotConfiguration:BotToken");
        }
        static IList<UserConnection> Users = new List<UserConnection>();
        static HttpClient client = new HttpClient();

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

            var botToken = _botToken;
            var api = new BotClient(botToken);
            api.SendMessage(message.ChatId, message.Content); // Send a message to user
        }

        public void SendPhotoToUser(Message message)
        {
            var reciever = Users.FirstOrDefault(x => x.UserId == message.Receiver);
            var connectionId = reciever == null ? "offlineUser" : reciever.ConnectionId;
            message.MessageDate = message.MessageDate.ToLocalTime();
            this.messageService.Add(message);
            conversationServiceQuery.Update(message.ChatId);
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

            string content = message.Text ?? "";
            if (message.Type == MessageType.Photo)
            {
                content = GetPhoto(message.Photo[^1].FileId);
            }

            Message msg = new Message
            {
                ChatId = message.Chat.Id,
                MessageDate = DateTime.Now,
                Content = content,
                ContentType = (int)message.Type,
                Receiver = Users.FirstOrDefault().UserId
            };
            messageService.Add(msg);

            var reciever = Users.FirstOrDefault(x => x.UserId == msg.Receiver);
            var connectionId = reciever == null ? "offlineUser" : reciever.ConnectionId;
            Clients.Client(connectionId).SendAsync("ReceiveDM", connectionId, msg);
        }

        public string GetPhoto(string photoID)
        {
            string photoApiUrl = string.Format("https://api.telegram.org/bot{0}/getFile?file_id={1}", _botToken, photoID);
            HttpResponseMessage response = Task.Run(async () => await client.GetAsync(photoApiUrl)).Result;
            string photoPath = "";
            if (response.IsSuccessStatusCode)
            {
                string resContent = Task.Run(async () => await response.Content.ReadAsStringAsync()).Result;
                TelegramPhotoResponse test = JsonConvert.DeserializeObject<TelegramPhotoResponse>(resContent);
                if (test.Ok)
                {
                    photoPath = test.Result.FilePath;
                }
            }

            if (!string.IsNullOrEmpty(photoPath))
            {
                return string.Format("https://api.telegram.org/file/bot{0}/{1}", _botToken, photoPath);
            }
            return "";
        }
    }
}
