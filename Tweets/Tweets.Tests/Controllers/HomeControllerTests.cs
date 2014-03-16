using System;
using System.Web.Mvc;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;
using Tweets.Controllers;
using Tweets.ModelBuilding;
using Tweets.Models;
using Tweets.Repositories;

namespace Tweets.Tests.Controllers
{
    public class HomeControllerTests : ControllerTestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            messageRepository = fixture.Freeze<IMessageRepository>();
            userReader = fixture.Freeze<IUserReader>();
            messageMapper = fixture.Freeze<IMapper<Message, MessageViewModel>>();
            userMessageMapper = fixture.Freeze<IMapper<UserMessage, MessageViewModel>>();
            userViewModelMapper = fixture.Freeze<IMapper<User, UserViewModel>>();
            sut = fixture.Create<HomeController>();
        }

        private HomeController sut;
        private IMessageRepository messageRepository;
        private IUserReader userReader;
        private IMapper<Message, MessageViewModel> messageMapper;
        private IMapper<UserMessage, MessageViewModel> userMessageMapper;
        private IMapper<User, UserViewModel> userViewModelMapper;

        [Test]
        public void Add_NonEmptyMessageOfKnownUser_ShouldSaveMessage()
        {
            var user = fixture.Create<User>();
            userReader.Stub(r => r.User).Return(user);
            var messageText = fixture.Create<string>();

            sut.Add(messageText);

            messageRepository.AssertWasCalled(r => r.Save(Arg<Message>.Matches(m => m.Text == messageText && m.User == user)));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Add_EmptyMessage_ShouldNotCallRepository(string message)
        {
            userReader.Stub(r => r.User).Return(fixture.Create<User>());
            sut.Add(message);
            messageRepository.AssertWasNotCalled(r => r.Save(Arg<Message>.Is.Anything));
        }

        [Test]
        public void Add_UnknownUser_ShouldNotCallRepository()
        {
            userReader.Stub(r => r.User).Return(null);
            sut.Add(fixture.Create<string>());
            messageRepository.AssertWasNotCalled(r => r.Save(Arg<Message>.Is.Anything));
        }

        [Test]
        public void Index_UnknownUser_ShouldReadTopMessages()
        {
            userReader.Stub(r => r.User).Return(null);
            messageRepository.Stub(r => r.GetPopularMessages()).Return(fixture.CreateMany<Message>());

            sut.Index();

            messageRepository.AssertWasCalled(r => r.GetPopularMessages());
        }

        [Test]
        public void Index_KnownUser_ShouldReadUserMessages()
        {
            var user = fixture.Create<User>();
            userReader.Stub(r => r.User).Return(user);
            messageRepository.Stub(r => r.GetMessages(Arg<User>.Is.Anything)).Return(fixture.CreateMany<UserMessage>());

            sut.Index();

            messageRepository.AssertWasCalled(r => r.GetMessages(user));
        }

        [Test]
        public void Index_GetPopularMessages_ShouldBuildModelByMapper()
        {
            var message1 = fixture.Create<Message>();
            var message2 = fixture.Create<Message>();
            userReader.Stub(r => r.User).Return(null);
            messageRepository.Stub(r => r.GetPopularMessages()).Return(new[] {message1, message2});

            sut.Index();

            messageMapper.AssertWasCalled(m => m.Map(message1));
            messageMapper.AssertWasCalled(m => m.Map(message2));
        }

        [Test]
        public void Index_GetMessages_ShouldBuildModelByMapper()
        {
            var message1 = fixture.Create<UserMessage>();
            var message2 = fixture.Create<UserMessage>();
            userReader.Stub(r => r.User).Return(fixture.Create<User>());
            messageRepository.Stub(r => r.GetMessages(Arg<User>.Is.Anything)).Return(new[] {message1, message2});

            sut.Index();

            userMessageMapper.AssertWasCalled(m => m.Map(message1));
            userMessageMapper.AssertWasCalled(m => m.Map(message2));
        }

        [Test]
        public void Index_Always_ShouldSetMessagesViewModelFromMapper()
        {
            userReader.Stub(r => r.User).Return(null);
            messageRepository.Stub(r => r.GetPopularMessages()).Return(new[] {fixture.Create<Message>()});
            var messageViewModel = fixture.Create<MessageViewModel>();
            messageMapper.Stub(m => m.Map(Arg<Message>.Is.Anything)).Return(messageViewModel);

            var actual = (HomePageViewModel) ((ViewResult) sut.Index()).Model;

            Assert.That(actual.Messages, Is.EqualTo(new[] {messageViewModel}));
        }

        [Test]
        public void Index_Always_ShouldSetUserViewModelFromMapper()
        {
            messageRepository.Stub(r => r.GetMessages(Arg<User>.Is.Anything)).Return(fixture.CreateMany<UserMessage>());
            var user = fixture.Create<User>();
            userReader.Stub(r => r.User).Return(user);
            var userViewModel = fixture.Create<UserViewModel>();
            userViewModelMapper.Stub(m => m.Map(user)).Return(userViewModel);

            var actual = (HomePageViewModel) ((ViewResult) sut.Index()).Model;

            Assert.That(actual.User, Is.EqualTo(userViewModel));
        }

        [Test]
        public void Like_UnknownUser_ShouldNotCallRepository()
        {
            userReader.Stub(r => r.User).Return(null);
            sut.Like(fixture.Create<Guid>());
            messageRepository.AssertWasNotCalled(r => r.Like(Arg<Guid>.Is.Anything, Arg<User>.Is.Anything));
        }

        [Test]
        public void Like_KnownUser_ShouldCallRepository()
        {
            var user = fixture.Create<User>();
            var messageId = fixture.Create<Guid>();
            userReader.Stub(r => r.User).Return(user);

            sut.Like(messageId);

            messageRepository.AssertWasCalled(r => r.Like(messageId, user));
        }

        [Test]
        public void Dislike_UnknownUser_ShouldNotCallRepository()
        {
            userReader.Stub(r => r.User).Return(null);
            sut.Dislike(fixture.Create<Guid>());
            messageRepository.AssertWasNotCalled(r => r.Dislike(Arg<Guid>.Is.Anything, Arg<User>.Is.Anything));
        }

        [Test]
        public void Dislike_KnownUser_ShouldCallRepository()
        {
            var user = fixture.Create<User>();
            var messageId = fixture.Create<Guid>();
            userReader.Stub(r => r.User).Return(user);

            sut.Dislike(messageId);

            messageRepository.AssertWasCalled(r => r.Dislike(messageId, user));
        }
    }
}