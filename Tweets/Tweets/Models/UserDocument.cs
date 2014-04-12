using System;

namespace Tweets.Models
{
    public class UserDocument
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public Uri ImageUrl { get; set; }
    }
}