using Tweets.Models;

namespace Tweets.ModelBuilding
{
    public class MessageDocumentMapper : IMapper<Message, MessageDocument>
    {
        public MessageDocument Map(Message message)
        {
            return new MessageDocument {Id = message.Id, Text = message.Text, UserName = message.User.Name, CreateDate = message.CreateDate};
        }
    }
}