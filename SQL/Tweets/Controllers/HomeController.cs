using System.Collections.Generic;
using System.Web.Mvc;
using Tweets.Models;

namespace Tweets.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var messages = new List<MessageViewModel>();
            var model = new HomeViewModel {TopMessages = messages};
            return View(model);
        }
    }
}