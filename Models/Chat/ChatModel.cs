﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Models.Chat
{
    public class ChatModel
    {
        public int Id { get; set; }

        public int MessageFrom { get; set; }

        public int MessageTo { get; set; }

        public string Type { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime DeletedAt { get; set; }
    }
}
