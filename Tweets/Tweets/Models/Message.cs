using System;

namespace Tweets.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public string Text { get; set; }
        public DateTime CreateDate { get; set; }
        public int Likes { get; set; }
    }
}