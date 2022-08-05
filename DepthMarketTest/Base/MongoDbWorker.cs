using MongoDB.Driver;

namespace DepthMarketTest.Base
{
    public class MongodbWorker<T> : IDbWorker<T>
    {
        private readonly ILogger<MongodbWorker<T>> _logger;
        private readonly IMongoDbContext<T> _context;

        public MongodbWorker(ILogger<MongodbWorker<T>> logger, IMongoDbContext<T> context)
        {
            _logger = logger;
            _context = context;
        }

        public Task<IEnumerable<T>> GetAllRecords()
        {
            return Task.FromResult<IEnumerable<T>>(_context);
        }

        public Task<IEnumerable<T>> GetRecordsByFilter(Func<T, bool> predicate)
        {
            var result = _context.Where(predicate);

            return Task.FromResult(result);
        }

        public async Task AddNewRecord(T record)
        {
            try
            {
                await _context.GetCollection().InsertOneAsync(record);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        public async Task AddNewRecordsRange(IEnumerable<T> records)
        {
            try
            {
                await _context.GetCollection().InsertManyAsync(records);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
  
            }
        }

        public async Task UpdateRecords(Func<T, bool> predicate, Action<T> updateFunc)
        {
            

            long totalUpdated = 0;

            try
            {
                var updatingItems = _context.Where(predicate);

                foreach (var updatingItem in updatingItems)
                {
                    var id = typeof(T).GetProperty("Id")?.GetValue(updatingItem);
                    var filter = Builders<T>.Filter.Eq("_id", id);

                    updateFunc(updatingItem);

                    var replacementResult = await _context.GetCollection().ReplaceOneAsync(filter, updatingItem);
                    totalUpdated += replacementResult.ModifiedCount;
                }

                _logger.LogInformation($"Updated {totalUpdated} records");
                
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        public async Task DeleteRecords(Func<T, bool> predicate)
        {
            

            long totalDeleted = 0;
            try
            {
                var deletingItems = _context.Where(predicate);

                foreach (var deletingItem in deletingItems)
                {
                    var id = typeof(T).GetProperty("Id")?.GetValue(deletingItem);
                    var filter = Builders<T>.Filter.Eq("_id", id);

                    var deletingResult = await _context.GetCollection().DeleteOneAsync(filter);
                    totalDeleted += deletingResult.DeletedCount;
                }

                _logger.LogInformation($"Deleted {totalDeleted} items");
                
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
    }
}
