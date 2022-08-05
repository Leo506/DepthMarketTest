namespace DepthMarketTest.Base
{
    public interface IDbContext<out T> : IEnumerable<T>
    {
    }
}
