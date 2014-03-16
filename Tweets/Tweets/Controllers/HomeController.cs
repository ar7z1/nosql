using System;
using System.Linq;
using System.Web.Mvc;
using Tweets.ModelBuilding;
using Tweets.Models;
using Tweets.Repositories;

namespace Tweets.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMapper<Message, MessageViewModel> messageMapper;
        private readonly IMessageRepository messageRepository;
        private readonly IMapper<UserMessage, MessageViewModel> userMessageMapper;
        private readonly IUserReader userReader;
        private readonly IMapper<User, UserViewModel> userViewModelMapper;

        public HomeController(IMessageRepository messageRepository,
                              IUserReader userReader,
                              IMapper<Message, MessageViewModel> messageMapper,
                              IMapper<UserMessage, MessageViewModel> userMessageMapper,
                              IMapper<User, UserViewModel> userViewModelMapper)
        {
            this.messageRepository = messageRepository;
            this.userReader = userReader;
            this.messageMapper = messageMapper;
            this.userMessageMapper = userMessageMapper;
            this.userViewModelMapper = userViewModelMapper;
        }

        public ActionResult Index()
        {
            var user = userReader.User;
            var model = user == null
                            ? messageRepository.GetPopularMessages().Select(messageMapper.Map)
                            : messageRepository.GetMessages(user).Select(userMessageMapper.Map);
            return View(new HomePageViewModel {Messages = model.ToArray(), User = userViewModelMapper.Map(user)});
        }

        [Authorize]
        [HttpPost]
        public ActionResult Like(Guid messageId)
        {
            var user = userReader.User;
            if (user != null)
                messageRepository.Like(messageId, user);
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public ActionResult Dislike(Guid messageId)
        {
            var user = userReader.User;
            if (user != null)
                messageRepository.Dislike(messageId, user);
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public ActionResult Add(string messageText)
        {
            var user = userReader.User;
            if (!string.IsNullOrWhiteSpace(messageText) && user != null)
            {
                var message = new Message {Text = messageText, CreateDate = DateTime.UtcNow, User = user, Id = Guid.NewGuid()};
                messageRepository.Save(message);
            }
            return RedirectToAction("Index");
        }
    }
}