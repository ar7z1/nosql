using Tweets.Models;

namespace Tweets.ModelBuilding
{
    public class MessageToMessageViewModelMapper : IMapper<Message, MessageViewModel>
    {
        public MessageViewModel Map(Message message)
        {
            return new MessageViewModel
                   {
                       MessageId = message.Id,
                       UserName = message.User.Name,
                       Content = message.Text,
                       Likes = message.Likes,
                       CreateDate = message.CreateDate
                   };
        }
    }
}