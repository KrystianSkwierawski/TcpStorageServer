namespace CodeCraftersRedis.Common.Models
{
    public class StorageValue
    {
        public required string Value { get; set; }

        public DateTime ExpiryDate { get; set; }
    }
}
