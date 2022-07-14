namespace Combinations.Tests
{
    [TestClass]
    public class PermutationsTests
    {
        public static void Equivalent(IEnumerable<string> ts1, IEnumerable<char[]> ts2)
        {
            var s1 = ts1.ToArray();
            var s2 = ts2.Select(t => new string(t)).ToArray();
            CollectionAssert.AreEquivalent(s1, s2);
        }

        [TestMethod]
        public void EmptyHasNoPermutations() => Equivalent(Enumerable.Empty<string>(), "".ToArray().AllPermutations());

        [TestMethod]
        public void SingleDigitHasOnePermutations() => Equivalent(new[] { "0" }, "0".ToArray().AllPermutations());

        [TestMethod]
        public void DoubleDigitHasTwoPermutations() => Equivalent(new[] { "01", "10" }, "01".ToArray().AllPermutations());

        [TestMethod]
        public void TrippleDigitHasNinePermutations() => Equivalent(new[] { "012", "021", "102", "120", "201", "210" }, "012".ToArray().AllPermutations());
    }
}