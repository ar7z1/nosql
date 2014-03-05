using System;
using System.Data.Linq.Mapping;

namespace Tweets.Models
{
    [Table(Name = "messages")]
    public class MessageDocument
    {
        [Column(Name = "id", DbType = "UniqueIdentifier", IsPrimaryKey = true, CanBeNull = false)]
        public Guid Id { get; set; }

        [Column(Name = "userName", DbType = "varchar(100)")]
        public string UserName { get; set; }

        [Column(Name = "text", DbType = "varchar(1000)")]
        public string Text { get; set; }

        [Column(Name = "createDate", DbType = "datetime")]
        public DateTime CreateDate { get; set; }

        [Column(Name = "version", DbType = "RowVersion", CanBeNull = false, IsVersion = true, IsDbGenerated = true)]
        public byte[] Version { get; protected set; }
    }
}