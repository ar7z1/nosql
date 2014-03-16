using System;
using Tweets.Attributes;

namespace Tweets.Models
{
    [BucketName("users")]
    public class UserDocument
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public Uri ImageUrl { get; set; }
    }
}