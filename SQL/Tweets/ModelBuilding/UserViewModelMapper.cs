using Tweets.Models;

namespace Tweets.ModelBuilding
{
    public class UserViewModelMapper : IMapper<User, UserViewModel>
    {
        public UserViewModel Map(User user)
        {
            if (user == null)
                return null;
            return new UserViewModel {DisplayName = user.DisplayName, ImageUrl = user.ImageUrl};
        }
    }
}