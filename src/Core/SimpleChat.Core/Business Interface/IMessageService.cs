using SimpleChat.Core.Entities;
using SimpleChat.Core.Model;
using System.Threading.Tasks;

namespace SimpleChat.Core.Business_Interface
{
    public interface IMessageService
    {
        void Add(Message message);
        void MarkAllMessagesAsRead(long chatId);
        Task<Message> DeleteMessage(MessageDeleteModel messageDeleteModel);
    }
}
