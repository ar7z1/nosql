using System;
using System.Configuration;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
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

            var mappingSource = new AttributeMappingSource();
            var connectionString = ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString;
            connection = new SqlConnection(connectionString);
            connection.Open();
            dataContext = new DataContext(connection, mappingSource);
            TruncateTable<LikeDocument>();
            TruncateTable<MessageDocument>();
            var messageDocumentMapper = fixture.Create<MessageDocumentMapper>();
            fixture.Inject<IMapper<Message, MessageDocument>>(messageDocumentMapper);
            sut = fixture.Create<MessageRepository>();
        }

        [TearDown]
        public void TearDown()
        {
            dataContext.Dispose();
            connection.Close();
            connection.Dispose();
        }

        private void TruncateTable<T>()
        {
            var tableName = typeof (T).GetCustomAttribute<TableAttribute>().Name;
            dataContext.ExecuteCommand(string.Format("if object_id('{0}') is not null delete from {0}", tableName));
            dataContext.SubmitChanges();
        }

        private SqlConnection connection;
        private DataContext dataContext;
        private MessageRepository sut;

        private void Insert<T>(T document) where T : class
        {
            dataContext.GetTable<T>().InsertOnSubmit(document);
            dataContext.SubmitChanges();
        }

        [Test]
        public void Save_NewMessage_InsertMessage()
        {
            var message = fixture.Create<Message>();
            sut.Save(message);

            var actual = dataContext.GetTable<MessageDocument>().ToArray();

            Assert.That(actual, Has.Some.Matches<MessageDocument>(m => m.Id == message.Id));
        }

        [Test]
        public void Like_NonExistingMessage_ShouldThrowError()
        {
            Assert.Throws<SqlException>(() => sut.Like(fixture.Create<Guid>(), fixture.Create<User>()));
        }

        [Test]
        public void Like_ExistingMessage_ShouldAddLikeDocument()
        {
            var messageDocument = fixture.Create<MessageDocument>();
            Insert(messageDocument);
            var user = fixture.Create<User>();
            sut.Like(messageDocument.Id, user);

            var actual = dataContext.GetTable<LikeDocument>().ToArray();

            Assert.That(actual, Has.Some.Matches<LikeDocument>(d => d.MessageId == messageDocument.Id && d.UserName == user.Name));
        }

        [Test]
        public void Like_LikedMessage_ShouldNotAddLikeDocument()
        {
            var messageDocument = fixture.Create<MessageDocument>();
            Insert(messageDocument);
            var user = fixture.Create<User>();
            var existingLike = new LikeDocument {MessageId = messageDocument.Id, UserName = user.Name, CreateDate = DateTime.UtcNow};
            Insert(existingLike);

            Assert.Throws<SqlException>(() => sut.Like(messageDocument.Id, user));
        }

        [Test]
        public void Dislike_NonExistingMessage_ShouldNotThrowError()
        {
            Assert.DoesNotThrow(() => sut.Dislike(fixture.Create<Guid>(), fixture.Create<User>()));
        }

        [Test]
        public void Dislike_ExistingMessage_ShouldRemoveLikeDocument()
        {
            var messageDocument = fixture.Create<MessageDocument>();
            Insert(messageDocument);
            var user = fixture.Create<User>();
            var existingLike = new LikeDocument {MessageId = messageDocument.Id, UserName = user.Name, CreateDate = DateTime.UtcNow};
            Insert(existingLike);

            sut.Dislike(messageDocument.Id, user);

            var actual = dataContext.GetTable<LikeDocument>().ToArray();

            Assert.That(actual, Has.No.Some.Matches<LikeDocument>(d => d.MessageId == messageDocument.Id && d.UserName == user.Name));
        }

        [Test]
        public void Dislike_SameUserOtherMessage_ShouldNotRemoveLikeDocument()
        {
            var messageDocument = fixture.Create<MessageDocument>();
            Insert(messageDocument);
            var user = fixture.Create<User>();
            var existingLike = new LikeDocument {MessageId = messageDocument.Id, UserName = user.Name, CreateDate = DateTime.UtcNow};
            Insert(existingLike);

            sut.Dislike(fixture.Create<Guid>(), user);

            var actual = dataContext.GetTable<LikeDocument>().ToArray();

            Assert.That(actual, Has.Some.Matches<LikeDocument>(d => d.MessageId == messageDocument.Id && d.UserName == user.Name));
        }

        [Test]
        public void Dislike_SameMessageOtherUser_ShouldNotRemoveLikeDocument()
        {
            var messageDocument = fixture.Create<MessageDocument>();
            Insert(messageDocument);
            var user = fixture.Create<User>();
            var existingLike = new LikeDocument {MessageId = messageDocument.Id, UserName = user.Name, CreateDate = DateTime.UtcNow};
            Insert(existingLike);

            sut.Dislike(messageDocument.Id, fixture.Create<User>());

            var actual = dataContext.GetTable<LikeDocument>().ToArray();

            Assert.That(actual, Has.Some.Matches<LikeDocument>(d => d.MessageId == messageDocument.Id && d.UserName == user.Name));
        }

        [Test]
        public void GetPopularMessages_MessageWithoutLike_ShouldReturnZeroLikes()
        {
            var messageDocument = fixture.Create<MessageDocument>();
            Insert(messageDocument);

            var actual = sut.GetPopularMessages();

            Assert.That(actual, Has.Some.Matches<Message>(m => m.Id == messageDocument.Id && m.Likes == 0));
        }

        [Test]
        public void GetPopularMessages_MessageWithLikes_ShouldSumLikes()
        {
            var messageDocument = fixture.Create<MessageDocument>();
            Insert(messageDocument);
            Insert(fixture.Build<LikeDocument>().With(d => d.MessageId, messageDocument.Id).Create());
            Insert(fixture.Build<LikeDocument>().With(d => d.MessageId, messageDocument.Id).Create());

            var actual = sut.GetPopularMessages();

            Assert.That(actual, Has.Some.Matches<Message>(m => m.Id == messageDocument.Id && m.Likes == 2));
        }

        [Test]
        public void GetPopularMessages_MessagesWithLikes_ShouldOrderByLikes()
        {
            var messageDocument1 = fixture.Create<MessageDocument>();
            Insert(messageDocument1);
            var messageDocument2 = fixture.Create<MessageDocument>();
            Insert(messageDocument2);
            Insert(fixture.Build<LikeDocument>().With(d => d.MessageId, messageDocument2.Id).Create());

            var actual = sut.GetPopularMessages();

            Assert.That(actual, Is.Ordered.By("Likes").Descending);
        }

        [Test]
        public void GetPopularMessages_Always_ShouldTakeTop10()
        {
            var messages = fixture.CreateMany<MessageDocument>(11);
            dataContext.GetTable<MessageDocument>().InsertAllOnSubmit(messages);
            dataContext.SubmitChanges();

            var actual = sut.GetPopularMessages();

            Assert.That(actual, Has.Length.EqualTo(10));
        }

        [Test]
        public void GetMessages_UnknownUser_ShouldReturnEmpty()
        {
            Insert(fixture.Create<MessageDocument>());
            var actual = sut.GetMessages(fixture.Create<User>());
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void GetMessages_KnownUser_ShouldReturnFilteredMessages()
        {
            var user = fixture.Create<User>();
            var messageDocument = fixture.Build<MessageDocument>().With(d => d.UserName, user.Name).Create();
            Insert(messageDocument);

            var actual = sut.GetMessages(user);

            Assert.That(actual, Has.Some.Matches<Message>(m => m.Id == messageDocument.Id));
        }

        [Test]
        public void GetMessages_MessageWithoutLike_ShouldReturnZeroLikes()
        {
            var user = fixture.Create<User>();
            var messageDocument = fixture.Build<MessageDocument>().With(d => d.UserName, user.Name).Create();
            Insert(messageDocument);

            var actual = sut.GetMessages(user);

            Assert.That(actual, Has.Some.Matches<Message>(m => m.Id == messageDocument.Id && m.Likes == 0));
        }

        [Test]
        public void GetMessages_MessageWithLikes_ShouldSumLikes()
        {
            var user = fixture.Create<User>();
            var messageDocument = fixture.Build<MessageDocument>().With(d => d.UserName, user.Name).Create();
            Insert(messageDocument);
            Insert(fixture.Build<LikeDocument>().With(d => d.MessageId, messageDocument.Id).Create());
            Insert(fixture.Build<LikeDocument>().With(d => d.MessageId, messageDocument.Id).Create());

            var actual = sut.GetMessages(user);

            Assert.That(actual, Has.Some.Matches<Message>(m => m.Id == messageDocument.Id && m.Likes == 2));
        }

        [Test]
        public void GetMessages_MessagesWithLikes_ShouldOrderByCreateDate()
        {
            var user = fixture.Create<User>();
            var messageDocument1 = fixture.Build<MessageDocument>().With(d => d.UserName, user.Name).With(d => d.CreateDate, DateTime.UtcNow).Create();
            Insert(messageDocument1);
            var messageDocument2 =
                fixture.Build<MessageDocument>().With(d => d.UserName, user.Name).With(d => d.CreateDate, DateTime.UtcNow.AddDays(1)).Create();
            Insert(messageDocument2);

            var actual = sut.GetMessages(user);

            Assert.That(actual, Is.Ordered.By("CreateDate").Descending);
        }

        [Test]
        public void GetMessages_CurrentUserLikedMessage_ShouldSetLikedToTrue()
        {
            var user = fixture.Create<User>();
            var messageDocument = fixture.Build<MessageDocument>().With(d => d.UserName, user.Name).Create();
            Insert(messageDocument);
            var likeDocument = fixture.Build<LikeDocument>().With(d => d.MessageId, messageDocument.Id).With(d => d.UserName, user.Name).Create();
            Insert(likeDocument);

            var actual = sut.GetMessages(user);

            Assert.That(actual, Has.Some.Matches<UserMessage>(m => m.Id == messageDocument.Id && m.Liked));
        }

        [Test]
        public void GetMessages_OtherUserLikedMessage_ShouldSetLikedToFalse()
        {
            var user = fixture.Create<User>();
            var messageDocument = fixture.Build<MessageDocument>().With(d => d.UserName, user.Name).Create();
            Insert(messageDocument);
            var likeDocument = fixture.Build<LikeDocument>().With(d => d.MessageId, messageDocument.Id).Create();
            Insert(likeDocument);

            var actual = sut.GetMessages(user);

            Assert.That(actual, Has.Some.Matches<UserMessage>(m => m.Id == messageDocument.Id && !m.Liked));
        }
    }
}