using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Tweets.ModelBuilding;
using Tweets.Models;
using Tweets.Repositories;

namespace Tweets.Tests.Repositories
{
    public class MessageRepositoryTests : TestBase
    {
        public override void SetUp()
        {
            base.SetUp();

            messagesCollection = GetCleanCollection();
            var messageDocumentMapper = fixture.Create<MessageDocumentMapper>();
            fixture.Inject<IMapper<Message, MessageDocument>>(messageDocumentMapper);
            sut = fixture.Create<MessageRepository>();
        }

        private MessageRepository sut;
        private MongoCollection<MessageDocument> messagesCollection;

        private MongoCollection<MessageDocument> GetCleanCollection()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
            var databaseName = MongoUrl.Create(connectionString).DatabaseName;
            var result =
                new MongoClient(connectionString).GetServer().GetDatabase(databaseName).GetCollection<MessageDocument>(MessageDocument.CollectionName);
            result.RemoveAll();
            return result;
        }

        private class MessagesByCreateDateComparer : IComparer<UserMessage>
        {
            public int Compare(UserMessage x, UserMessage y)
            {
                return Comparer<DateTime>.Default.Compare(x.CreateDate, y.CreateDate);
            }
        }

        [Test]
        public void Dislike_ExistingMessage_ShouldRemoveLike()
        {
            var user = fixture.Create<User>();
            var existingLike = new LikeDocument {UserName = user.Name, CreateDate = DateTime.UtcNow};
            var messageDocument = fixture.Build<MessageDocument>().With(d => d.Likes, new[] {existingLike}).Create();
            messagesCollection.Save(messageDocument);

            sut.Dislike(messageDocument.Id, user);

            var actual = messagesCollection.FindOneAs<MessageDocument>(Query<MessageDocument>.EQ(d => d.Id, messageDocument.Id));

            Assert.That(actual.Likes, Is.Empty);
        }

        [Test]
        public void Dislike_NonExistingMessage_ShouldNotThrowError()
        {
            Assert.DoesNotThrow(() => sut.Dislike(fixture.Create<Guid>(), fixture.Create<User>()));
        }

        [Test]
        public void Dislike_AnotherUserLike_ShouldNotRemoveLike()
        {
            var user = fixture.Create<User>();
            var existingLike = new LikeDocument {UserName = user.Name, CreateDate = DateTime.UtcNow};
            var messageDocument = fixture.Build<MessageDocument>().With(d => d.Likes, new[] {existingLike}).Create();
            messagesCollection.Save(messageDocument);

            sut.Dislike(messageDocument.Id, fixture.Create<User>());

            var actual = messagesCollection.FindOneAs<MessageDocument>(Query<MessageDocument>.EQ(d => d.Id, messageDocument.Id));

            Assert.That(actual.Likes, Has.Some.Matches<LikeDocument>(d => d.UserName == user.Name));
        }

        [Test]
        public void Dislike_SameUserOtherMessage_ShouldNotRemoveLike()
        {
            var user = fixture.Create<User>();
            var like = new LikeDocument {UserName = user.Name, CreateDate = DateTime.UtcNow};
            var messageDocument = fixture.Build<MessageDocument>().With(d => d.Likes, new[] {like}).Create();
            messagesCollection.Save(messageDocument);

            sut.Dislike(fixture.Create<Guid>(), user);

            var actual = messagesCollection.FindOneAs<MessageDocument>(Query<MessageDocument>.EQ(d => d.Id, messageDocument.Id));

            Assert.That(actual.Likes, Has.Some.Matches<LikeDocument>(d => d.UserName == user.Name));
        }

        [Test]
        public void GetMessages_CurrentUserLikedMessage_ShouldSetLikedToTrue()
        {
            var user = fixture.Create<User>();
            var like = fixture.Build<LikeDocument>().With(d => d.UserName, user.Name).Create();
            var messageDocument = fixture.Build<MessageDocument>().With(d => d.UserName, user.Name).With(d => d.Likes, new[] {like}).Create();
            messagesCollection.Insert(messageDocument);

            var actual = sut.GetMessages(user);

            Assert.That(actual, Has.Some.Matches<UserMessage>(m => m.Id == messageDocument.Id && m.Liked));
        }

        [Test]
        public void GetMessages_KnownUser_ShouldReturnFilteredMessages()
        {
            var user = fixture.Create<User>();
            var messageDocument = fixture.Build<MessageDocument>().With(d => d.UserName, user.Name).Create();
            messagesCollection.Insert(messageDocument);

            var actual = sut.GetMessages(user);

            Assert.That(actual, Has.Some.Matches<Message>(m => m.Id == messageDocument.Id));
        }

        [Test]
        public void GetMessages_MessageWithLikes_ShouldSumLikes()
        {
            var user = fixture.Create<User>();
            var messageDocument =
                fixture.Build<MessageDocument>().With(d => d.UserName, user.Name).With(d => d.Likes, fixture.CreateMany<LikeDocument>(2)).Create();
            messagesCollection.Insert(messageDocument);

            var actual = sut.GetMessages(user);

            Assert.That(actual, Has.Some.Matches<Message>(m => m.Id == messageDocument.Id && m.Likes == 2));
        }

        [Test]
        public void GetMessages_MessageWithoutLike_ShouldReturnZeroLikes()
        {
            var user = fixture.Create<User>();
            var messageDocument = fixture.Build<MessageDocument>().With(d => d.UserName, user.Name).Without(d => d.Likes).Create();
            messagesCollection.Insert(messageDocument);

            var actual = sut.GetMessages(user);

            Assert.That(actual, Has.Some.Matches<Message>(m => m.Id == messageDocument.Id && m.Likes == 0));
        }

        [Test]
        public void GetMessages_MessagesWithLikes_ShouldOrderByCreateDate()
        {
            var user = fixture.Create<User>();
            var messageDocument1 = fixture.Build<MessageDocument>().With(d => d.UserName, user.Name).With(d => d.CreateDate, DateTime.UtcNow).Create();
            messagesCollection.Insert(messageDocument1);
            var messageDocument2 =
                fixture.Build<MessageDocument>().With(d => d.UserName, user.Name).With(d => d.CreateDate, DateTime.UtcNow.AddDays(1)).Create();
            messagesCollection.Insert(messageDocument2);

            var actual = sut.GetMessages(user);

            Assert.That(actual, Is.Ordered.Using(new MessagesByCreateDateComparer()).Descending);
        }

        [Test]
        public void GetMessages_OtherUserLikedMessage_ShouldSetLikedToFalse()
        {
            var user = fixture.Create<User>();
            var messageDocument = fixture.Build<MessageDocument>().With(d => d.UserName, user.Name).Create();
            messagesCollection.Insert(messageDocument);

            var actual = sut.GetMessages(user);

            Assert.That(actual, Has.Some.Matches<UserMessage>(m => m.Id == messageDocument.Id && !m.Liked));
        }

        [Test]
        public void GetMessages_UnknownUser_ShouldReturnEmpty()
        {
            messagesCollection.Insert(fixture.Create<MessageDocument>());
            var actual = sut.GetMessages(fixture.Create<User>());
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void GetPopularMessages_Always_ShouldTakeTop10()
        {
            var messages = fixture.CreateMany<MessageDocument>(11);
            messagesCollection.InsertBatch(messages);

            var actual = sut.GetPopularMessages();

            Assert.That(actual.ToArray(), Has.Length.EqualTo(10));
        }

        [Test]
        public void GetPopularMessages_MessageWithLikes_ShouldSumLikes()
        {
            var messageDocument = fixture.Build<MessageDocument>().With(d => d.Likes, fixture.CreateMany<LikeDocument>(2)).Create();
            messagesCollection.Insert(messageDocument);

            var actual = sut.GetPopularMessages();

            Assert.That(actual, Has.Some.Matches<Message>(m => m.Id == messageDocument.Id && m.Likes == 2));
        }

        [Test]
        public void GetPopularMessages_MessageWithoutLike_ShouldReturnZeroLikes()
        {
            var messageDocument = fixture.Build<MessageDocument>().Without(d => d.Likes).Create();
            messagesCollection.Insert(messageDocument);

            var actual = sut.GetPopularMessages();

            Assert.That(actual, Has.Some.Matches<Message>(m => m.Id == messageDocument.Id && m.Likes == 0));
        }

        [Test]
        public void GetPopularMessages_MessagesWithLikes_ShouldOrderByLikes()
        {
            var messageDocument1 = fixture.Build<MessageDocument>().With(d => d.Likes, fixture.CreateMany<LikeDocument>(1)).Create();
            messagesCollection.Insert(messageDocument1);
            var messageDocument2 = fixture.Build<MessageDocument>().With(d => d.Likes, fixture.CreateMany<LikeDocument>(3)).Create();
            messagesCollection.Insert(messageDocument2);

            var actual = sut.GetPopularMessages();

            Assert.That(actual, Is.Ordered.By("Likes").Descending);
        }

        [Test]
        public void GetPopularMessages_Always_ShouldSortBeforeTop()
        {
            messagesCollection.InsertBatch(fixture.Build<MessageDocument>().With(d => d.Likes, fixture.CreateMany<LikeDocument>(1)).CreateMany(10));
            var messageDocument = fixture.Build<MessageDocument>().With(d => d.Likes, fixture.CreateMany<LikeDocument>(10)).Create();
            messagesCollection.Insert(messageDocument);

            var actual = sut.GetPopularMessages();

            Assert.That(actual.First().Id, Is.EqualTo(messageDocument.Id));
        }

        [Test]
        public void Like_ExistingMessage_ShouldAddLikeDocument()
        {
            var messageDocument = fixture.Create<MessageDocument>();
            messagesCollection.Insert(messageDocument);
            var user = fixture.Create<User>();
            sut.Like(messageDocument.Id, user);

            var actual = messagesCollection.FindOneAs<MessageDocument>(Query<MessageDocument>.EQ(d => d.Id, messageDocument.Id));

            Assert.That(actual.Likes, Has.Some.Matches<LikeDocument>(d => d.UserName == user.Name));
        }

        [Test]
        public void Like_LikedMessage_ShouldNotAddLikeDocument()
        {
            var user = fixture.Create<User>();
            var like = new LikeDocument {UserName = user.Name, CreateDate = DateTime.UtcNow};
            var messageDocument = fixture.Build<MessageDocument>().With(d => d.Likes, new[] {like}).Create();
            messagesCollection.Insert(messageDocument);

            sut.Like(messageDocument.Id, user);

            var actual = messagesCollection.FindOneAs<MessageDocument>(Query<MessageDocument>.EQ(d => d.Id, messageDocument.Id));
            Assert.That(actual.Likes.ToArray(), Has.Length.EqualTo(1));
        }

        [Test]
        public void Like_LikedMessage_ShouldNotRemoveOtherUserLikes()
        {
            var user = fixture.Create<User>();
            var likes = fixture.CreateMany<LikeDocument>();
            var messageDocument = fixture.Build<MessageDocument>().With(d => d.Likes, likes).Create();
            messagesCollection.Insert(messageDocument);

            sut.Like(messageDocument.Id, user);

            var actual = messagesCollection.FindOneAs<MessageDocument>(Query<MessageDocument>.EQ(d => d.Id, messageDocument.Id));
            Assert.That(actual.Likes.Count(), Is.EqualTo(likes.Count() + 1));
        }

        [Test]
        public void Like_NonExistingMessage_ShouldNotAddMessageDocument()
        {
            sut.Like(fixture.Create<Guid>(), fixture.Create<User>());
            var actual = messagesCollection.Count();
            Assert.That(actual, Is.EqualTo(0));
        }

        [Test]
        public void Like_UnknownMessageId_ShouldDoNothing()
        {
            var messageDocument = fixture.Create<MessageDocument>();
            messagesCollection.Insert(messageDocument);

            sut.Like(fixture.Create<Guid>(), fixture.Create<User>());

            var actual = messagesCollection.FindAll();

            Assert.That(actual.Count(), Is.EqualTo(1));
            Assert.That(actual.Single().Likes.Count(), Is.EqualTo(messageDocument.Likes.Count()));
        }

        [Test]
        public void Save_NewMessage_InsertMessage()
        {
            var message = fixture.Create<Message>();
            sut.Save(message);

            var actual = messagesCollection.FindAll();

            Assert.That(actual, Has.Some.Matches<MessageDocument>(m => m.Id == message.Id));
        }
    }
}