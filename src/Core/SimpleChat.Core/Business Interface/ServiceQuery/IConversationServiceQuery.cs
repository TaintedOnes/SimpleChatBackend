using SimpleChat.Core.Entities;
using System.Collections.Generic;

namespace SimpleChat.Core.Business_Interface.ServiceQuery
{
    public interface IConversationServiceQuery
    {
        IEnumerable<Conversation> GetAllConversation();
        void Add(Conversation conversation);
    }
}
