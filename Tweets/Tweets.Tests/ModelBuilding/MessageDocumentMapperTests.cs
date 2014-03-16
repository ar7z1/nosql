using System;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Tweets.ModelBuilding;
using Tweets.Models;

namespace Tweets.Tests.ModelBuilding
{
    public class MessageDocumentMapperTests : TestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            sut = fixture.Create<MessageDocumentMapper>();
        }

        private MessageDocumentMapper sut;

        [Test]
        public void Map_TrivialMappings_ShouldMapCorrectly()
        {
            var id = fixture.Create<Guid>();
            var user = fixture.Create<User>();
            var text = fixture.Create<string>();
            var createDate = fixture.Create<DateTime>();
            var message =
                fixture.Build<Message>().With(m => m.Id, id).With(m => m.User, user).With(m => m.Text, text).With(m => m.CreateDate, createDate).Create();

            var actual = sut.Map(message);

            Assert.That(actual.Id, Is.EqualTo(id));
            Assert.That(actual.UserName, Is.EqualTo(user.Name));
            Assert.That(actual.Text, Is.EqualTo(text));
            Assert.That(actual.CreateDate, Is.EqualTo(createDate));
        }
    }
}