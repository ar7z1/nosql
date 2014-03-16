using System.Reflection;
using CorrugatedIron;
using CorrugatedIron.Models;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;
using Tweets.Attributes;
using Tweets.ModelBuilding;
using Tweets.Models;
using Tweets.Repositories;

namespace Tweets.Tests.Repositories
{
    public class UserRepositoryTests : TestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            cluster = RiakCluster.FromConfig("riakConfig");
            client = cluster.CreateClient();
            fixture.Inject(client);
            userDocumentMapper = fixture.Freeze<IMapper<User, UserDocument>>();
            userMapper = fixture.Freeze<IMapper<UserDocument, User>>();
            sut = fixture.Create<UserRepository>();
        }

        [TearDown]
        public void TearDown()
        {
            cluster.Dispose();
        }

        private UserRepository sut;
        private IRiakClient client;
        private IRiakEndPoint cluster;
        private IMapper<User, UserDocument> userDocumentMapper;
        private IMapper<UserDocument, User> userMapper;

        private string UsersBucketName
        {
            get
            {
                return typeof (UserDocument).GetCustomAttribute<BucketNameAttribute>().BucketName;
            }
        }

        [Test]
        public void Save_NewUser_ShouldInsert()
        {
            var user = fixture.Create<User>();
            var userDocument = fixture.Create<UserDocument>();
            userDocumentMapper.Stub(m => m.Map(user)).Return(userDocument);

            sut.Save(user);

            var actual = client.Get(UsersBucketName, userDocument.Id).Value.GetObject<UserDocument>();
            Assert.That(actual.Id, Is.EqualTo(userDocument.Id));
            Assert.That(actual.DisplayName, Is.EqualTo(userDocument.DisplayName));
            Assert.That(actual.ImageUrl, Is.EqualTo(userDocument.ImageUrl));
        }

        [Test]
        public void Save_ExistingUser_ShouldUpdate()
        {
            var userId = fixture.Create<string>();
            var existingUser = fixture.Build<UserDocument>().With(d => d.Id, userId).Create();
            client.Put(new RiakObject(UsersBucketName, userId, existingUser));
            var newUser = fixture.Build<UserDocument>().With(d => d.Id, userId).Create();
            userDocumentMapper.Stub(m => m.Map(Arg<User>.Is.Anything)).Return(newUser);

            sut.Save(fixture.Create<User>());

            var actual = client.Get(UsersBucketName, userId).Value.GetObject<UserDocument>();
            Assert.That(actual.DisplayName, Is.EqualTo(newUser.DisplayName));
            Assert.That(actual.ImageUrl, Is.EqualTo(newUser.ImageUrl));
        }

        [Test]
        public void Get_NonExistingUser_ShouldReturnNull()
        {
            userMapper.Stub(m => m.Map(Arg<UserDocument>.Is.Anything)).Return(fixture.Create<User>());
            var actual = sut.Get(fixture.Create<string>());
            Assert.That(actual, Is.Null);
        }

        [Test]
        public void Get_ExistingUser_ShouldReturnResultFromMapper()
        {
            var userId = fixture.Create<string>();
            var existingUser = fixture.Build<UserDocument>().With(d => d.Id, userId).Create();
            client.Put(new RiakObject(UsersBucketName, userId, existingUser));
            var user = fixture.Create<User>();
            userMapper.Stub(m => m.Map(Arg<UserDocument>.Matches(d => d.Id == userId))).Return(user);

            var actual = sut.Get(userId);

            Assert.That(actual, Is.EqualTo(user));
        }
    }
}