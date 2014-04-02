using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tweets.Models
{
    public class MessageDocument
    {
        public const string CollectionName = "messages";

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonElement("userName")]
        public string UserName { get; set; }

        [BsonElement("text")]
        public string Text { get; set; }

        [BsonElement("createDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateDate { get; set; }

        [BsonElement("likes")]
        public IEnumerable<LikeDocument> Likes { get; set; }
    }
}