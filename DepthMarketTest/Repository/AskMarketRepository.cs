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
        
        public async Task<List<MarketModel>> GetRelevantAsksAsync(OrderModel model)
        {
            var filter = Builders<MarketModel>.Filter.Lte("price", model.Price);
            // по id товара
            // price  меньше или равно
            // дата самые старые - самые первые
            await _asksCollection.FindAsync(filter);
            throw new NotImplementedException();
        }
    }
}
