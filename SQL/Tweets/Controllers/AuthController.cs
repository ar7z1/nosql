using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;

namespace Tweets.Controllers
{
    public class AuthController : Controller
    {
        [AllowAnonymous]
        public void Login(string returnUrl)
        {
            OAuthWebSecurity.RequestAuthentication("Twitter", Url.Action("LoginCallback", new {returnUrl}));
        }

        [AllowAnonymous]
        public ActionResult LoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication();
            if (!result.IsSuccessful)
                return RedirectToAction(Url.Action("Index", "Home"));

            FormsAuthentication.SetAuthCookie(result.UserName, false);
            return Redirect(returnUrl);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect(Url.Action("Index", "Home"));
        }
    }
}