using NUnit.Framework;

using static StS.Helpers;

namespace StS.PermutationTests
{
    public class PermutationTests
    {
        [SetUp]
        public void Setup()
        {
            Helpers.SetRandom(0);
        }

        [Test]
        public void Test_Permutations()
        {
            var cis = GetCis("Strike", "Strike+");
            var permutations = GenSubsets(cis, 1);
            Assert.AreEqual(2, permutations.Count);
        }

        [Test]
        public void Test_Permutations2()
        {
            var cis = GetCis("Strike", "Inflame", "Dazed", "Clash", "Clash");
            var permutations = GenSubsets(cis, 1);
            Assert.AreEqual(5, permutations.Count);

            var permutations2 = GenSubsets(cis, 2);
            Assert.AreEqual(10, permutations2.Count);
        }

        [Test]
        public void Test_Permutations3()
        {
            var cis = GetCis("Strike", "Inflame", "Dazed", "Clash", "Clash", "Clash", "Headbutt+");
            var permutations = GenSubsets(cis, 1);
            Assert.AreEqual(7, permutations.Count);

            var permutations2 = GenSubsets(cis, 2);
            Assert.AreEqual(21, permutations2.Count);

            var permutations3 = GenSubsets(cis, 3);
            Assert.AreEqual(35, permutations3.Count);
        }

        [Test]
        public void Test_Weighting()
        {
            var cis = GetCis("Strike", "Inflame", "Dazed", "Clash", "Clash", "Havok", "Havok");
            var permutations = GenSubsets(cis, 2);
            var weights = GenHandWeights(permutations);

            //TODO this needs validation.
            Assert.AreEqual(12, weights.Count);
        }
    }
}
