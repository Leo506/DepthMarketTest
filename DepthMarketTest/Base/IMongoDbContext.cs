using MongoDB.Driver;

namespace DepthMarketTest.Base
{
    public interface IMongoDbContext<T> : IDbContext<T>
    {
        IMongoCollection<T> GetCollection();
    }
}
