using System.Collections.Generic;

namespace Myggot.Classes
{
    public class AccountCache
    {
        private Dictionary<int, Dictionary<string, object>> cache;

        public AccountCache()
        {
            cache = new Dictionary<int, Dictionary<string, object>>();
        }

        public void AddToCache(int accountId, string key, object data)
        {
            if (!cache.ContainsKey(accountId))
            {
                cache[accountId] = new Dictionary<string, object>();
            }

            cache[accountId][key] = data;
        }

        public object GetFromCache(int accountId, string key)
        {
            if (cache.ContainsKey(accountId) && cache[accountId].ContainsKey(key))
            {
                return cache[accountId][key];
            }
            return null;
        }

        public bool IsCached(int accountId, string key)
        {
            return cache.ContainsKey(accountId) && cache[accountId].ContainsKey(key);
        }
    }
}
