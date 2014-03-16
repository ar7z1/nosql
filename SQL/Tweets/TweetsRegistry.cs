using CorrugatedIron;
using StructureMap.Configuration.DSL;

namespace Tweets
{
    public class TweetsRegistry : Registry
    {
        public TweetsRegistry()
        {
            For<IRiakClient>().Singleton().Use(c => RiakCluster.FromConfig("riakConfig").CreateClient());
        }
    }
}