using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq.Mapping;
using System.Linq;
using Tweets.ModelBuilding;
using Tweets.Models;

namespace Tweets.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly string connectionString;
        private readonly AttributeMappingSource mappingSource;
        private readonly IMapper<Message, MessageDocument> messageDocumentMapper;

        public MessageRepository(IMapper<Message, MessageDocument> messageDocumentMapper)
        {
            this.messageDocumentMapper = messageDocumentMapper;
            mappingSource = new AttributeMappingSource();
            connectionString = ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString;
        }

        public void Save(Message message)
        {
            var messageDocument = messageDocumentMapper.Map(message);
            //TODO: Здесь нужно реализовать вставку сообщения в базу
        }

        public void Like(Guid messageId, User user)
        {
            var likeDocument = new LikeDocument {MessageId = messageId, UserName = user.Name, CreateDate = DateTime.UtcNow};
            //TODO: Здесь нужно реализовать вставку одобрения в базу
        }

        public void Dislike(Guid messageId, User user)
        {
            //TODO: Здесь нужно реализовать удаление одобрения из базы
        }

        public IEnumerable<Message> GetPopularMessages()
        {
            //TODO: Здесь нужно возвращать 10 самых популярных сообщений
            return Enumerable.Empty<Message>();
        }

        public IEnumerable<UserMessage> GetMessages(User user)
        {
            //TODO: Здесь нужно получать все сообщения конкретного пользователя
            return Enumerable.Empty<UserMessage>();
        }
    }
}