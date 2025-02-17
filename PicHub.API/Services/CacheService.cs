using Microsoft.Extensions.Caching.Memory;

namespace PicHub.API.Services
{
    public interface ICacheService
    {
        Task SetBucketName(string email, string bucketName);
        Task<string> GetBucketName(string email);
    }

    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(5);

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }
        public async Task SetBucketName(string email, string bucketName)
        {
            _cache.Set(email, bucketName, _defaultExpiration);
        }
        public async Task<string> GetBucketName(string email)
        {
            if (_cache.TryGetValue(email, out string bucketName))
            {
                _cache.Remove(email);
                return bucketName;
            }
            return null;
        }
    }
}
