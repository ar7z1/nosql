using Microsoft.Web.WebPages.OAuth;

namespace Tweets
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            OAuthWebSecurity.RegisterTwitterClient("lIvRx7BJZ4xdLffZcR1ygw", "zMtfxNahFHfUDiVTsVoKoYvDC89t4DEyP2b6qGZcSg");
        }
    }
}