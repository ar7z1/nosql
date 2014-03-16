using Tweets.Models;

namespace Tweets.Repositories
{
    public interface IUserRepository
    {
        void Save(User user);
        User Get(string userName);
    }
}