using Tweets.Models;

namespace Tweets.Controllers
{
    public interface IUserReader
    {
        User User { get; }
    }
}