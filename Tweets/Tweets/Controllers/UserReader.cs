using System.Web;
using Tweets.Models;
using Tweets.Repositories;

namespace Tweets.Controllers
{
    public class UserReader : IUserReader
    {
        private readonly IUserRepository userRepository;

        public UserReader(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public User User
        {
            get
            {
                if (string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name))
                    return null;
                return userRepository.Get(HttpContext.Current.User.Identity.Name);
            }
        }
    }
}