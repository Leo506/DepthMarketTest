using DepthMarketTest.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DepthMarketTest.Repository
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly IMongoCollection<OrderModel> _ordersCollection;

        public OrdersRepository(IMongoDatabase mongoDatabase)
        {
            _ordersCollection = mongoDatabase.GetCollection<OrderModel>("orders");

        }
        public async Task<string> CreateNewAsync(OrderModel newOrder)
        {
            await _ordersCollection.InsertOneAsync(newOrder);
            return newOrder.Id;
        }
        public async Task<OrderModel> GetByIdAsync(string id)
        {
            return await _ordersCollection.Find(_ => _.Id == id).FirstOrDefaultAsync();
        }
        public async Task<List<OrderModel>> GetAllAsync()
        {
            return await _ordersCollection.Find(_ => true).ToListAsync();
        }
        public async Task UpdateAsync(OrderModel orderToUpdate)
        {
            await _ordersCollection.ReplaceOneAsync(x => x.Id == orderToUpdate.Id, orderToUpdate);
        }
        public async Task DeleteAsync(string id)
        {
            await _ordersCollection.DeleteOneAsync(x => x.Id == id);
        }
    }
}
