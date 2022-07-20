namespace Combinations.Tests
{
    [TestClass]
    public class PairTests
    {
        [TestMethod]
        public void NoPairsOfEmpty() => CollectionAssert.AreEquivalent(new string[] { }.Pairs().ToArray(), new (string first, string second)[0]);

        [TestMethod]
        public void NoPairsOfOne() => CollectionAssert.AreEquivalent(new [] { "0" }.Pairs().ToArray(), new (string first, string second)[0]);

        [TestMethod]
        public void OnePairOfTwo() => CollectionAssert.AreEquivalent(new[] { "0", "1" }.Pairs().ToArray(), new (string first, string second)[] { ("0", "1") });

        [TestMethod]
        public void ThreePairsOfThree() => CollectionAssert.AreEquivalent(new[] { "0", "1", "2" }.Pairs().ToArray(), new (string first, string second)[] { ("0", "1"), ("0", "2"), ("1", "2") });

        [TestMethod]
        public void SixPairsOfFour() => CollectionAssert.AreEquivalent(new[] { "0", "1", "2", "3" }.Pairs().ToArray(), new (string first, string second)[] { ("0", "1"), ("0", "2"), ("0", "3"), ("1", "2"), ("1", "3"), ("2", "3") });
    }
}