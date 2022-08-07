using DepthMarketTest.Models;
using MongoDB.Driver;

namespace DepthMarketTest.Repository
{
    public class BidMarketRepository : IBidMarketRepository
    {
        private readonly IMongoCollection<MarketModel> _asksCollection;

        public BidMarketRepository(IMongoDatabase mongoDatabase)
        {
            _asksCollection = mongoDatabase.GetCollection<MarketModel>("market_bid");
            if (!_asksCollection.Indexes.List().Any())
            {
                var indexKeysDefine = Builders<MarketModel>.IndexKeys
                    .Ascending(x => x.ProductId)
                    .Descending(x => x.Price)
                    .Ascending(x => x.SubmissionTime);
                _asksCollection.Indexes.CreateOne(new CreateIndexModel<MarketModel>(indexKeysDefine));
            }
        }

        public async Task<string> CreateNewAsync(MarketModel bid)
        {
            await _asksCollection.InsertOneAsync(bid);
            return bid.Id;
        }
        public async Task<MarketModel> GetByIdAsync(string id)
        {
            return await _asksCollection.Find(_ => _.Id == id).FirstOrDefaultAsync();
        }
        public async Task<List<MarketModel>> GetAllAsync()
        {
            return await _asksCollection.Find(_ => true).ToListAsync();
        }
        public async Task UpdateAsync(MarketModel bidToUpdate)
        {
            await _asksCollection.ReplaceOneAsync(x => x.Id == bidToUpdate.Id, bidToUpdate);
        }
        public async Task DeleteAsync(string id)
        {
            await _asksCollection.DeleteOneAsync(x => x.Id == id);
        }

        public async Task<List<MarketModel>> GetRelevantBidsAsync(OrderModel model)
        {
            throw new NotImplementedException();
        }
    }
}

