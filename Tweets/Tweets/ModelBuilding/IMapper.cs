namespace Tweets.ModelBuilding
{
    public interface IMapper<TSource, TDestination>
    {
        TDestination Map(TSource source);
    }
}