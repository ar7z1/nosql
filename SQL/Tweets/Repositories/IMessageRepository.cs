using System;
using System.Collections.Generic;
using Tweets.Models;

namespace Tweets.Repositories
{
    public interface IMessageRepository
    {
        void Save(Message message);
        void Like(Guid messageId, User user);
        void Dislike(Guid messageId, User user);
        IEnumerable<Message> GetPopularMessages();
        IEnumerable<UserMessage> GetMessages(User user);
    }
}