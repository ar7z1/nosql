using System;
using System.Collections.Generic;
using Tweets.Models;

namespace Tweets.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        public void Save(Message message)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Message> GetMessages(User user)
        {
            throw new NotImplementedException();
        }
    }
}