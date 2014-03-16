using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoRhinoMock;

namespace Tweets.Tests
{
    [TestFixture]
    public abstract class TestBase
    {
        [SetUp]
        public virtual void SetUp()
        {
            fixture = new Fixture();
            fixture.Customize(new AutoRhinoMockCustomization());
        }

        protected Fixture fixture;
    }
}