using System.Collections.Generic;

namespace Tweets.Models
{
    public class HomeViewModel
    {
        public IEnumerable<MessageViewModel> TopMessages { get; set; }
    }
}