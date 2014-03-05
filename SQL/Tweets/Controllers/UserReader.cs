using System.Web;
using Tweets.Models;

namespace Tweets.Controllers
{
    public class UserReader : IUserReader
    {
        public User User
        {
            get
            {
                if (string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name))
                    return null;
                return new User {Name = HttpContext.Current.User.Identity.Name};
            }
        }
    }
}