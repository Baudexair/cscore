using System;
using System.Collections.Generic;
using System.Linq;

namespace com.csutil {

    public static class IEnumerableExtensions {

        public static IEnumerable<R> Map<T, R>(this IEnumerable<T> self, Func<T, R> selector) {
            return self.Select(selector);
        }

        public static T Reduce<T>(this IEnumerable<T> self, Func<T, T, T> func) {
            return self.Aggregate(func);
        }

        public static R Reduce<T, R>(this IEnumerable<T> self, R seed, Func<R, T, R> func) {
            return self.Aggregate<T, R>(seed, func);
        }

        /// <summary> Takes an input collection of type A where each A has a collection of type B. 
        /// Combines these collections to a single result collection of type B </summary>
        public static IEnumerable<B> ReduceToUnion<A, B>(this IEnumerable<A> self, Func<A, IEnumerable<B>> func) {
            return ReduceToUnion(self, new List<B>(), func);
        }

        /// <summary> Takes an input collection of type A where each A has a collection of type B. 
        /// Combines these collections to a single result collection of type B </summary>
        public static IEnumerable<B> ReduceToUnion<A, B>(this IEnumerable<A> self, IEnumerable<B> seed, Func<A, IEnumerable<B>> func) {
            return self.Aggregate(seed, (r, t) => r.AddRangeViaUnion(func(t)));
        }

        public static IEnumerable<T> Filter<T>(this IEnumerable<T> self, Func<T, bool> predicate) {
            return self.Where(predicate);
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> self) { return self == null || !self.Any(); }

        public static string ToStringV2(this IEnumerable<string> args, string bracket1 = "[", string bracket2 = "]") {
            return args.ToStringV2<string>(x => x, bracket1, bracket2);
        }

        public static string ToStringV2<T>(this IEnumerable<T> args, Func<T, string> toString, string bracket1 = "[", string bracket2 = "]") {
            if (args == null) { return "null"; }
            if (args.IsNullOrEmpty()) { return bracket1 + bracket2; }
            var filteredResultStrings = args.Map(x => "" + toString(x)).Filter(x => !x.IsNullOrEmpty());
            if (filteredResultStrings.IsNullOrEmpty()) { return bracket1 + bracket2; }
            return bracket1 + filteredResultStrings.Reduce((x, y) => x + ", " + y) + bracket2;
        }

        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source) {
            var r = new HashSet<TSource>();
            foreach (var e in source) { r.Add(e); }
            return r;
        }

        public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<K> keys, IEnumerable<V> values) {
            return keys.Zip(values, (key, value) => new KeyValuePair<K, V>(key, value)).ToDictionary(p => p.Key, p => p.Value);
        }

        public static int IndexOf<T>(this IEnumerable<T> self, T elem) { return self.IndexOf(x => Equals(x, elem)); }

        public static int IndexOf<T>(this IEnumerable<T> self, Func<T, bool> predicate) {
            var index = -1;
            if (self.Any(x => { index++; return predicate(x); })) { return index; }
            return -1;
        }

    }

}