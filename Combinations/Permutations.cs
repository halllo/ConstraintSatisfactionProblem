using System;
using System.Collections.Generic;
using System.Linq;

namespace Combinations
{
    public static class Permutations
    {
        public static IEnumerable<T[]> AllPermutations<T>(this T[] ts)
        {
            if (ts.Length == 1)
            {
                yield return ts;
            }
            else
            {
                for (int i = 0; i < ts.Length; i++)
                {
                    var rest = ts.SelectMany((t, index) => index != i ? new[] { t } : Array.Empty<T>()).ToArray();
                    foreach (var permutations in rest.AllPermutations())
                    {
                        yield return new[] { ts[i] }.Concat(permutations).ToArray();
                    }
                }
            }
        }

        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> ts)
        {
            var random = new Random();
            return ts.OrderBy(t => random.Next());
        }

        public static IEnumerable<(T first, T second)> Pairs<T>(this IList<T> ts)
        {
            for (int i = 0; i < ts.Count; i++)
            {
                for (int j = i + 1; j < ts.Count; j++)
                {
                    yield return (ts[i], ts[j]);
                }
            }
        }
    }
}