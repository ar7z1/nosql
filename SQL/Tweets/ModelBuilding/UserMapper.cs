using Tweets.Models;

namespace Tweets.ModelBuilding
{
    public class UserMapper : IMapper<UserDocument, User>
    {
        public User Map(UserDocument document)
        {
            return new User {Name = document.Id, DisplayName = document.DisplayName, ImageUrl = document.ImageUrl};
        }
    }
}