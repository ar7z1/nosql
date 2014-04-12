using System;
using ServiceStack.Redis;
using Tweets.ModelBuilding;
using Tweets.Models;

namespace Tweets.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly RedisClient redisClient;
        private readonly IMapper<User, UserDocument> userDocumentMapper;
        private readonly IMapper<UserDocument, User> userMapper;

        public UserRepository(RedisClient redisClient, IMapper<User, UserDocument> userDocumentMapper, IMapper<UserDocument, User> userMapper)
        {
            this.redisClient = redisClient;
            this.userDocumentMapper = userDocumentMapper;
            this.userMapper = userMapper;
        }

        public void Save(User user)
        {
            //TODO: Здесь нужно реализовать сохранение пользователя в Redis
        }

        public User Get(string userName)
        {
            //TODO: Здесь нужно доставать пользователя из Redis
            return new User
                   {
                       Name = userName,
                       DisplayName = "Какой-то пользователь",
                       ImageUrl = new Uri("http://www.kagms.ru/upload/medialibrary/b3a/no-image-icon-md.jpg")
                   };
        }
    }
}