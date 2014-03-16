using System;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Tweets.ModelBuilding;
using Tweets.Models;

namespace Tweets.Tests.ModelBuilding
{
    public class UserViewModelMapperTests : TestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            sut = fixture.Create<UserViewModelMapper>();
        }

        private UserViewModelMapper sut;

        [Test]
        public void Map_TrivialMappings_ShouldMapCorrectly()
        {
            var displayName = fixture.Create<string>();
            var imageUrl = fixture.Create<Uri>();
            var user = fixture.Build<User>().With(u => u.DisplayName, displayName).With(u => u.ImageUrl, imageUrl).Create();

            var actual = sut.Map(user);

            Assert.That(actual.DisplayName, Is.EqualTo(displayName));
            Assert.That(actual.ImageUrl, Is.EqualTo(imageUrl));
        }

        [Test]
        public void Map_NullUser_ShouldReturnNull()
        {
            var actual = sut.Map(null);
            Assert.That(actual, Is.Null);
        }
    }
}