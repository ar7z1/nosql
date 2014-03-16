using System;
using System.Data.Linq.Mapping;

namespace Tweets.Models
{
    [Table(Name = "likes")]
    public class LikeDocument
    {
        [Column(Name = "userName", DbType = "varchar(100)", IsPrimaryKey = true)]
        public string UserName { get; set; }

        [Column(Name = "messageId", DbType = "UniqueIdentifier", IsPrimaryKey = true, CanBeNull = false)]
        public Guid MessageId { get; set; }

        [Column(Name = "createDate", DbType = "datetime")]
        public DateTime CreateDate { get; set; }
    }
}