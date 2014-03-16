using System;

namespace Tweets.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class BucketNameAttribute : Attribute
    {
        public BucketNameAttribute(string bucketName)
        {
            BucketName = bucketName;
        }

        public string BucketName { get; private set; }
    }
}