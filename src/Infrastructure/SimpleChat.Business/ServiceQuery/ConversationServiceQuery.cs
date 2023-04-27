using SimpleChat.Core.Business_Interface.ServiceQuery;
using SimpleChat.Core.Entities;
using SimpleChat.Core.Repository_Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleChat.Business.ServiceQuery
{
    public class ConversationServiceQuery : IConversationServiceQuery
    {
        private readonly IUnitOfWork unitOfWork;
        public ConversationServiceQuery(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IEnumerable<Conversation> GetAllConversation()
        {
            try
            {
                var conversations = this.unitOfWork.Repository<Conversation>().Get().ToList();
                return conversations;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void Add(Conversation conversation)
        {
            unitOfWork.Repository<Conversation>().Add(conversation);
            unitOfWork.SaveChanges();
        }

        public void Update(long chatId)
        {
            // Find the conversation by its ChatId
            var conversation = unitOfWork.Repository<Conversation>().Get(c => c.ChatId == chatId).FirstOrDefault();

            // If conversation is found, update the LastMessageDate property
            if (conversation != null)
            {
                conversation.LastMessageDate = DateTime.Now;

                // Call the Update method to update the conversation in the database
                unitOfWork.Repository<Conversation>().Update(conversation);
            }
            unitOfWork.SaveChanges();
        }
    }
}