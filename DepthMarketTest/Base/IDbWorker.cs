namespace DepthMarketTest.Base
{
    public interface IDbWorker<T>
    {
        Task<IEnumerable<T>> GetAllRecords();

        Task<IEnumerable<T>> GetRecordsByFilter(Func<T, bool> predicate);

        Task AddNewRecord(T record);

        Task AddNewRecordsRange(IEnumerable<T> records);

        Task UpdateRecords(Func<T, bool> predicate, Action<T> updateFunc);

        Task DeleteRecords(Func<T, bool> predicate);
    }
}
