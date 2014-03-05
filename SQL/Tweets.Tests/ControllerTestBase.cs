using System.Web.Mvc;

namespace Tweets.Tests
{
    public abstract class ControllerTestBase : TestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            fixture.Customize<ControllerContext>(c => c.Without(x => x.DisplayMode));
        }
    }
}