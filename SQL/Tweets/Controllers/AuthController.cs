using System;
using System.Web.Mvc;
using System.Web.Security;
using Tweets.Models;
using Tweets.Repositories;
using TweetSharp;

namespace Tweets.Controllers
{
    public class AuthController : Controller
    {
        private const string ConsumerKey = "lIvRx7BJZ4xdLffZcR1ygw";
        private const string ConsumerSecret = "zMtfxNahFHfUDiVTsVoKoYvDC89t4DEyP2b6qGZcSg";
        private readonly IUserRepository userRepository;

        public AuthController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            var service = new TwitterService(ConsumerKey, ConsumerSecret);
            var requestToken = service.GetRequestToken(Url.Action("LoginCallback", "Auth", new {returnUrl}, Request.Url.Scheme));
            var uri = service.GetAuthorizationUri(requestToken);
            return Redirect(uri.ToString());
        }

        [AllowAnonymous]
        public ActionResult LoginCallback(string returnUrl, string oauth_token, string oauth_verifier)
        {
            var requestToken = new OAuthRequestToken {Token = oauth_token};
            var service = new TwitterService(ConsumerKey, ConsumerSecret);
            var accessToken = service.GetAccessToken(requestToken, oauth_verifier);
            service.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);
            var twitterUser = service.VerifyCredentials(new VerifyCredentialsOptions());

            var user = new User {Name = twitterUser.ScreenName, DisplayName = twitterUser.Name, ImageUrl = new Uri(twitterUser.ProfileImageUrl)};
            userRepository.Save(user);
            FormsAuthentication.SetAuthCookie(user.Name, false);
            return Redirect(returnUrl);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect(Url.Action("Index", "Home"));
        }
    }
}