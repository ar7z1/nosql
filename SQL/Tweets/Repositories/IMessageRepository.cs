using System.Collections.Generic;
using Tweets.Models;

namespace Tweets.Repositories
{
    public interface IMessageRepository
    {
        void Save(Message message);
        IEnumerable<Message> GetMessages(User user);
    }
}