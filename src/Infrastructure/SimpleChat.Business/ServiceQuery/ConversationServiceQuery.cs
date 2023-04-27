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
            this.unitOfWork.Repository<Conversation>().Add(conversation);
            this.unitOfWork.SaveChanges();
        }
    }
}