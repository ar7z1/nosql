using System;

namespace Tweets.Models
{
    public class Message
    {
        public User User { get; set; }
        public string Text { get; set; }
        public DateTime PublishDate { get; set; }
        public int Likes { get; set; }
    }
}