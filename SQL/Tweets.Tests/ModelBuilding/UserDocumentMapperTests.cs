using System;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Tweets.ModelBuilding;
using Tweets.Models;

namespace Tweets.Tests.ModelBuilding
{
    public class UserDocumentMapperTests : TestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            sut = fixture.Create<UserDocumentMapper>();
        }

        private UserDocumentMapper sut;

        [Test]
        public void Map_TrivialMappings_ShouldMapCorrectly()
        {
            var id = fixture.Create<string>();
            var displayName = fixture.Create<string>();
            var imageUrl = fixture.Create<Uri>();
            var user = fixture.Build<User>().With(u => u.Name, id).With(u => u.DisplayName, displayName).With(u => u.ImageUrl, imageUrl).Create();

            var actual = sut.Map(user);

            Assert.That(actual.Id, Is.EqualTo(id));
            Assert.That(actual.DisplayName, Is.EqualTo(displayName));
            Assert.That(actual.ImageUrl, Is.EqualTo(imageUrl));
        }
    }
}