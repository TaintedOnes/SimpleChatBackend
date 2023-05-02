using Microsoft.EntityFrameworkCore;
using SimpleChat.Core.Business_Interface;
using SimpleChat.Core.Entities;
using SimpleChat.Core.Enums;
using SimpleChat.Core.Model;
using SimpleChat.Core.Repository_Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleChat.Business
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork unitOfWork;
        public MessageService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public void Add(Message message)
        {
            this.unitOfWork.Repository<Message>().Add(message);
            this.unitOfWork.SaveChanges();
        }
        public void MarkAllMessagesAsRead(long chatId)
        {
            var messages = unitOfWork.Repository<Message>().Get()
                .Where(m => m.ChatId == chatId && m.IsNew == true)
                .ToList();
            foreach (var message in messages)
            {
                message.IsNew = false;
                unitOfWork.Repository<Message>().Update(message);
            }
            unitOfWork.SaveChanges();
        }

        async Task<Message> IMessageService.DeleteMessage(MessageDeleteModel messageDeleteModel)
        {
            // var message = messageDeleteModel.Message;
            var messageRepo = this.unitOfWork.Repository<Message>();
            var message = await messageRepo.Get().Where(x => x.Id == messageDeleteModel.Message.Id).FirstOrDefaultAsync();
            if (messageDeleteModel.DeleteType == DeleteTypeEnum.DeleteForEveryone.ToString())
            {
                message.IsReceiverDeleted = true;
                message.IsSenderDeleted = true;
            }
            else
            {
                message.IsReceiverDeleted = message.IsReceiverDeleted || (message.Receiver == messageDeleteModel.DeletedUserId);
                message.IsSenderDeleted = message.IsSenderDeleted || (message.Sender == messageDeleteModel.DeletedUserId);
            }
            messageRepo.Update(message);
            await this.unitOfWork.SaveChangesAsync();
            return message;
        }
    }
}
