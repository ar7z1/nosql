using System.Collections.Generic;

namespace Tweets.Models
{
    public class HomePageViewModel
    {
        public IEnumerable<MessageViewModel> Messages { get; set; }
        public UserViewModel User { get; set; }
    }
}