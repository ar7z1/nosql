using System;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Tweets.ModelBuilding;
using Tweets.Models;

namespace Tweets.Tests.ModelBuilding
{
    public class UserMapperTests : TestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            sut = fixture.Create<UserMapper>();
        }

        private UserMapper sut;

        [Test]
        public void Map_TrivialMappings_ShouldMapCorrectly()
        {
            var name = fixture.Create<string>();
            var displayName = fixture.Create<string>();
            var imageUrl = fixture.Create<Uri>();
            var user = fixture.Build<UserDocument>().With(u => u.Id, name).With(u => u.DisplayName, displayName).With(u => u.ImageUrl, imageUrl).Create();

            var actual = sut.Map(user);

            Assert.That(actual.Name, Is.EqualTo(name));
            Assert.That(actual.DisplayName, Is.EqualTo(displayName));
            Assert.That(actual.ImageUrl, Is.EqualTo(imageUrl));
        }
    }
}