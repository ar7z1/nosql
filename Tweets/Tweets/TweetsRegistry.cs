using System;
using System.Configuration;
using ServiceStack.Redis;
using StructureMap.Configuration.DSL;

namespace Tweets
{
    public class TweetsRegistry : Registry
    {
        public TweetsRegistry()
        {
            For<RedisClient>().Use(c =>
                                   {
                                       var connectionString = ConfigurationManager.ConnectionStrings["Redis"].ConnectionString;
                                       return new RedisClient(new Uri(connectionString));
                                   });
        }
    }
}