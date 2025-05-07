namespace TcpStorageServer.Common.Models
{
    public class StorageValue
    {
        public required string Value { get; init; }

        public DateTime ExpiryDate { get; init; }
    }
}
