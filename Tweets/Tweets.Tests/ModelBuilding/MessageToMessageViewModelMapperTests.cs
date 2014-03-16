using System;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Tweets.ModelBuilding;
using Tweets.Models;

namespace Tweets.Tests.ModelBuilding
{
    public class MessageToMessageViewModelMapperTests : TestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            sut = fixture.Create<MessageToMessageViewModelMapper>();
        }

        private MessageToMessageViewModelMapper sut;

        [Test]
        public void Map_TrivialMappings_ShouldMapCorrectly()
        {
            var messageId = fixture.Create<Guid>();
            var user = fixture.Create<User>();
            var content = fixture.Create<string>();
            var likes = fixture.Create<int>();
            var createDate = fixture.Create<DateTime>();
            var message =
                fixture.Build<Message>()
                       .With(m => m.Id, messageId)
                       .With(m => m.User, user)
                       .With(m => m.Text, content)
                       .With(m => m.Likes, likes)
                       .With(m => m.CreateDate, createDate)
                       .Create();

            var actual = sut.Map(message);

            Assert.That(actual.MessageId, Is.EqualTo(messageId));
            Assert.That(actual.UserName, Is.EqualTo(user.Name));
            Assert.That(actual.Content, Is.EqualTo(content));
            Assert.That(actual.Likes, Is.EqualTo(likes));
            Assert.That(actual.CreateDate, Is.EqualTo(createDate));
        }
    }
}