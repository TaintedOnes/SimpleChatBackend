using SimpleChat.Core.Entities;
using SimpleChat.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat.Core.Business_Interface
{
    public interface IMessageService
    {
        void Add(Message message);
        Task<Message> DeleteMessage(MessageDeleteModel messageDeleteModel);
    }
}
