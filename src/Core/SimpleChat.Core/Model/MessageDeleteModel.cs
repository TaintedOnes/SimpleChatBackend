﻿using SimpleChat.Core.Entities;

namespace SimpleChat.Core.Model
{
    public class MessageDeleteModel
    {
        public string DeleteType { get; set; }
        public Message Message { get; set; }
        public string DeletedUserId { get; set; }
    }
}