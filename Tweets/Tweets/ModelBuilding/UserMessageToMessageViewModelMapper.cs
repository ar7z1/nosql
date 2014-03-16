using Tweets.Models;

namespace Tweets.ModelBuilding
{
    public class UserMessageToMessageViewModelMapper : IMapper<UserMessage, MessageViewModel>
    {
        public MessageViewModel Map(UserMessage message)
        {
            return new MessageViewModel
                   {
                       MessageId = message.Id,
                       UserName = message.User.Name,
                       Content = message.Text,
                       Likes = message.Likes,
                       CreateDate = message.CreateDate,
                       Liked = message.Liked
                   };
        }
    }
}