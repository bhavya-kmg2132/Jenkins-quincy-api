namespace Entities
{
    public class DeletedInMemoryCacheLog
    {
        public string CacheKey { get; set; }
        public DateTime DeletionTimeInUTC { get; set; }
    }
}
