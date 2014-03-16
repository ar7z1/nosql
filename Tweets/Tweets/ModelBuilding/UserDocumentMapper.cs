using Tweets.Models;

namespace Tweets.ModelBuilding
{
    public class UserDocumentMapper : IMapper<User, UserDocument>
    {
        public UserDocument Map(User user)
        {
            return new UserDocument {Id = user.Name, DisplayName = user.DisplayName, ImageUrl = user.ImageUrl};
        }
    }
}