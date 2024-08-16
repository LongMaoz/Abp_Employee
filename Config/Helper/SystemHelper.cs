using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config
{
    public static class SystemHelper
    {
        public static void AddHelper<T>(this List<T> list, T item)
        {
            if (item != null)
                list.Add(item);
        }
        public static void AddHelper<T>(this List<T> list, bool condition, T item)
        {
            if (condition && item != null)
                list.Add(item);
        }
        public static void AddRangeHelper<T>(this List<T> list, IEnumerable<T> collection)
        {
            if (collection != null && collection.Count() > 0)
                list.AddRange(collection);
        }
        public static void AddRangeHelper<T>(this List<T> list, bool condition, IEnumerable<T> collection)
        {
            if (condition && collection != null && collection.Count() > 0)
                list.AddRange(collection);
        }
        public static async Task ForEachAsync<T>(this List<T> list, Func<T, Task> fun)
        {
            foreach (var item in list)
            {
                await fun.Invoke(item);
            }
        }
        public static async Task<List<K>> SelectAsync<T, K>(this List<T> list, Func<T, Task<K>> fun) where K : class
        {
            List<K> results = new();
            foreach (var item in list)
            {
                var result = await fun.Invoke(item);
                if (result != null)
                    results.Add(result);
            }
            return results;
        }
        public static StringBuilder Append(this StringBuilder sb, bool condition, string value)
        {
            if (condition)
                sb.Append(value);
            return sb;
        }
    }
}
