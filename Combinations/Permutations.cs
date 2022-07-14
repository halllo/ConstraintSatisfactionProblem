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
    }
}