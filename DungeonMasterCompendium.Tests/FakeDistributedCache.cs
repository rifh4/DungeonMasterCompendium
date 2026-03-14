using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace DungeonMasterCompendium.Tests
{
    public class FakeDistributedCache : IDistributedCache
    {
        private readonly Dictionary<string, byte[]> _entries = new();

        public int SetCallCount { get; set; }
        public string? LastSetKey { get; set; }
        public DistributedCacheEntryOptions? LastSetOptions { get; set; }

        public byte[]? Get(string key)
        {
            if (_entries.TryGetValue(key, out byte[]? value))
            {
                return value;
            }

            return null;
        }

        public Task<byte[]?> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            byte[]? value = Get(key);
            return Task.FromResult(value);
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            _entries[key] = value;
            SetCallCount++;
            LastSetKey = key;
            LastSetOptions = options;
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default)
        {
            Set(key, value, options);
            return Task.CompletedTask;
        }

        public void Refresh(string key)
        {
        }

        public Task RefreshAsync(string key, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Remove(string key)
        {
            _entries.Remove(key);
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            Remove(key);
            return Task.CompletedTask;
        }

        public string? GetStoredString(string key)
        {
            byte[]? value = Get(key);
            if (value == null)
            {
                return null;
            }

            return Encoding.UTF8.GetString(value);
        }
    }
}