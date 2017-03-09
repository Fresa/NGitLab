using System.Collections.Generic;
using System.Collections.Specialized;

namespace NGitLab.Impl
{
    internal static class NameValueCollectionExtensions
    {
        internal static IDictionary<string, IEnumerable<string>> ToDictionary(this NameValueCollection collection)
        {
            var dictionary = new Dictionary<string, IEnumerable<string>>();
            foreach (var key in collection.AllKeys)
            {
                dictionary[key] = collection.GetValues(key);
            }
            return dictionary;
        }
    }
}