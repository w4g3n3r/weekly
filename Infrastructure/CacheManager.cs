using System.Text;
using System.Text.RegularExpressions;
using Weekly.Infrastructure.Cacheables;

namespace Weekly.Infrastructure
{
    public class CacheManager
    {
        private const string cacheExtension = ".cache";
        private const string cacheKeyPattern = @"^([^\.]*)\.(.*)$";
        private const string cacheEntryPattern = @"^([^:]*):(.*)$";

        private Dictionary<string, Dictionary<string, string>> _cache;

        public CacheManager()
        {
            _cache = new Dictionary<string, Dictionary<string, string>>();
        }

        public IEnumerable<T> GetValues<T>(string superKey)
            where T : ICacheable, new()
        {
            if (string.IsNullOrEmpty(superKey))
                throw new ArgumentNullException(nameof(superKey));

            LoadSuperKey(superKey);

            foreach (string key in _cache[superKey].Keys)
            {
                var value = _cache[superKey][key];
                var cachedValue = new T();
                cachedValue.FromCacheString(value);
                yield return cachedValue;
            }
        }

        public bool TryGetValue<T>(string key, out T value)
            where T : ICacheable, new()
        {
            value = default;

            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            (var superKey, var subKey) = GetCacheKey(key);

            if (superKey == null || subKey == null)
                throw new InvalidOperationException("Cache key was not property formatted.");

            LoadSuperKey(superKey);

            if (_cache[superKey].ContainsKey(subKey))
            {
                var cacheValue = _cache[superKey][subKey];

                value = new T();
                value.FromCacheString(cacheValue);

                return true;
            }

            return false;
        }

        public void SetValue<T>(string key, T value)
            where T : ICacheable
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            (var superKey, var subKey) = GetCacheKey(key);

            if (superKey == null || subKey == null)
                throw new InvalidOperationException("Cache key was not property formatted.");

            LoadSuperKey(superKey);

            _cache[superKey][subKey] = value.ToCacheString();
        }

        public void Persist()
        {
            foreach (var superKey in _cache.Keys)
            {
                SaveSuperKey(superKey);
            }
        }

        private (string, string) GetCacheKey(string key)
        {
            var cacheKey = Regex.Match(key.ToLower(), cacheKeyPattern);

            if (cacheKey.Success)
            {
                return (cacheKey.Groups[1].Value, cacheKey.Groups[2].Value);
            }
            else
            {
                return (null, null);
            }

        }

        private void SaveSuperKey(string superKey)
        {
            if (!_cache.ContainsKey(superKey))
            {
                return;
            }

            var cacheFilePath = GetCacheFilePath(superKey);

            using (var sw = new StreamWriter(cacheFilePath, false, Encoding.UTF8))
            {
                foreach (var subKey in _cache[superKey].Keys)
                {
                    sw.WriteLine($"{subKey}:{_cache[superKey][subKey]}");
                }

                sw.Flush();
                sw.Close();
            }
        }

        private void LoadSuperKey(string superKey)
        {
            if (_cache.ContainsKey(superKey))
            {
                // Super key is already loaded.
                return;
            }

            _cache.Add(superKey, new Dictionary<string, string>());

            var cacheFilePath = GetCacheFilePath(superKey);

            if (!File.Exists(cacheFilePath))
            {
                return;
            }

            using (var sr = new StreamReader(cacheFilePath, Encoding.UTF8))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();

                    if (!string.IsNullOrEmpty(line))
                    {
                        var cacheEntry = Regex.Match(line, cacheEntryPattern);

                        if (cacheEntry.Success)
                        {
                            var subKey = cacheEntry.Groups[1].Value;
                            var value = cacheEntry.Groups[2].Value;

                            _cache[superKey].Add(subKey, value);
                        }
                    }
                }
                sr.Close();
            }
        }

        private string GetCacheFilePath(string superKey)
        {
            return Path.Combine(Path.GetDirectoryName(Configuration.Path), $"{superKey}{cacheExtension}");
        }
    }
}
