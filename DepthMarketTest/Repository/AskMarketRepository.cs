using DepthMarketTest.Models;
using MongoDB.Driver;

namespace DepthMarketTest.Repository
{
    public class AskMarketRepository : IAskMarketRepository
    {
        private readonly IMongoCollection<MarketModel> _asksCollection;

        public AskMarketRepository(IMongoDatabase mongoDatabase)
        {
            _asksCollection = mongoDatabase.GetCollection<MarketModel>("market_ask");
            if (!_asksCollection.Indexes.List().Any())
            {
                var indexKeysDefine = Builders<MarketModel>.IndexKeys
                    .Ascending(x => x.ProductId)
                    .Ascending(x => x.Price)
                    .Ascending(x => x.SubmissionTime);
                _asksCollection.Indexes.CreateOne(new CreateIndexModel<MarketModel>(indexKeysDefine));
            }
        }
        public async Task<string> CreateNewAsync(MarketModel ask)
        {
            await _asksCollection.InsertOneAsync(ask);
            return ask.Id;
        }
        public async Task<MarketModel> GetByIdAsync(string id)
        {
            return await _asksCollection.Find(_ => _.Id == id).FirstOrDefaultAsync();
        }
        public async Task<List<MarketModel>> GetAllAsync()
        {
            return await _asksCollection.Find(_ => true).ToListAsync();
        }
        public async Task UpdateAsync(MarketModel askToUpdate)
        {
            await _asksCollection.ReplaceOneAsync(x => x.Id == askToUpdate.Id, askToUpdate);
        }
        public async Task DeleteAsync(string id)
        {
            await _asksCollection.DeleteOneAsync(x => x.Id == id);
        }

        public Task<List<MarketModel>> GetRelevantBidsAsync(OrderModel model)
        {
            throw new NotImplementedException();
        }
    }
}
