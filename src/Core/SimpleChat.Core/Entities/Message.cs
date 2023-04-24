using System;

namespace SimpleChat.Core.Entities
{
    public class Message
    {
        public string Id { get; set; }
        public string ChatId { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public DateTime MessageDate { get; set; }
        public string Content { get; set; }
        public bool IsNew { get; set; }
        public bool IsSenderDeleted { get; set; }
        public bool IsReceiverDeleted { get; set; }
    }
}
