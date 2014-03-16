using System;

namespace Tweets.Models
{
    public class MessageViewModel
    {
        public Guid MessageId { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public int Likes { get; set; }
        public DateTime CreateDate { get; set; }
        public bool Liked { get; set; }
    }
}