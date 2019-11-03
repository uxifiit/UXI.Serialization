using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXI.Serialization.Extensions
{
    public static class IDictionaryEx
    {
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return GetOrDefault(dictionary, key, default(TValue));
        }

        
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value)
                 ? value
                 : defaultValue;
        }


        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValueFactory)
        {
            if (defaultValueFactory == null)
            {
                throw new ArgumentNullException(nameof(defaultValueFactory), "Factory for default value must not be null.");
            }

            TValue value;
            return dictionary.TryGetValue(key, out value)
                 ? value
                 : defaultValueFactory.Invoke();
        }
    }
}
