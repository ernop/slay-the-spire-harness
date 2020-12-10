
using NUnit.Framework;

using static StS.Helpers;

namespace StS
{
    public class SimTests
    {
        [SetUp]
        public void Setup()
        {
            SetRandom(0);
        }

        //[Test]
        public void Test_Basic()
        {
            //I want to validate that I am properly generating all outcomes.
            var cis = GetCis("Strike", "Defend");

            var enemy = new Cultist(5, 5);
            var player = new Player();
            var fs = new FightSimulator(cis, enemy, player);
            var results = fs.Sim();

            var ii = 0;
            foreach (var fight in results)
            {

            }
        }

        [Test]
        public void Test_Planning1()
        {
            //set up fight where deck is SSSSS, SSDDD and draw last 5 first
            //enemy has 18hp
            //first round you should fullblock
            //because you always kill 2nd round
        }

        [Test]
        public void Test_Defending()
        {
            var cis = GetCis("Defend", "Defend", "Defend", "Defend", "Strike");

            var enemy = new GenericEnemy(10, 1, 8, 8);
            var player = new Player();
            var fs = new FightSimulator(cis, enemy, player);
            var res = fs.Sim();
            foreach (var initialHandRes in res)
            {
                Assert.AreEqual(100, initialHandRes.GetValue());
            }
            //set up fight where you have DDDDS, SSDDD and draw last 5 first
            //enemy has 18hp
            //enemy does mega damage after 2rd round
            //first round you should full attack and take the damage
            //because you always kill 2nd round
        }


        [Test]
        public void Test_PartialDefending()
        {
            var cis = GetCis("Defend", "Defend", "Defend", "Defend", "Strike");

            var enemy = new GenericEnemy(11, 1, 8, 8);
            var player = new Player();
            var fs = new FightSimulator(cis, enemy, player);
            var res = fs.Sim();
            foreach (var initialHandRes in res)
            {
                Assert.AreEqual(99, initialHandRes.GetValue());
            }
            //set up fight where you have DDDDS, SSDDD and draw last 5 first
            //enemy has 18hp
            //enemy does mega damage after 2rd round
            //first round you should full attack and take the damage
            //because you always kill 2nd round
        }

        [Test]
        public void Test_Planning2()
        {
            //set up fight where you have DDDDS, SSDDD and draw last 5 first
            //enemy has 18hp
            //enemy does mega damage after 2rd round
            //first round you should full attack and take the damage
            //because you always kill 2nd round
        }

        /// <summary>
        /// Test that we find the way to play inflame before strike to kill enemy before he kills us.
        /// </summary>
        [Test]
        public void Test_Using_Inflame()
        {
            var cis = GetCis("Strike", "Defend", "Inflame", "Defend", "Defend");

            var enemy = new GenericEnemy(4, 4, 8, 8);
            var player = new Player();
            var fs = new FightSimulator(cis, enemy, player);
            var res = fs.Sim();
            foreach (var initialHandRes in res)
            {
                Assert.AreEqual(100, initialHandRes.GetValue());
            }
        }
    }
}