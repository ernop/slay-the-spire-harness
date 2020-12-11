
using System.Collections.Generic;

using NUnit.Framework;

using static StS.Helpers;

namespace StS.Tests
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
            var fs = new FightSimulator(cis, enemy, player, doOutput: true);
            var node = fs.Sim();

            Assert.AreEqual(100, node.GetValue());

            //set up fight where you have DDDDS, SSDDD and draw last 5 first
            //enemy has 18hp
            //enemy does mega damage after 2rd round
            //first round you should full attack and take the damage
            //because you always kill 2nd round
        }

        [Test]
        public void Test_UsingPotionAndInflameDefending()
        {
            var cis = GetCis("Defend", "Defend", "Inflame", "Defend", "Strike");

            var enemy = new GenericEnemy(15, 3, 10, 10);
            var player = new Player(potions: new List<Potion>() { new StrengthPotion() });
            var fs = new FightSimulator(cis, enemy, player);
            var node = fs.Sim();

            Assert.AreEqual(100, node.GetValue());
        }


        [Test]
        public void Test_PartialDefending()
        {
            var cis = GetCis("Defend", "Defend", "Defend", "Defend", "Strike");

            var enemy = new GenericEnemy(11, 1, 8, 8);
            var player = new Player();
            var fs = new FightSimulator(cis, enemy, player);
            var node = fs.Sim();

            Assert.AreEqual(99, node.GetValue());

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
            var node = fs.Sim();

            Assert.AreEqual(100, node.GetValue());

        }

        [Test]
        public void Test_ExploringDrawSpace()
        {
            var cis = GetCis("Strike", "Defend");
            var enemy = new GenericEnemy(5, 1, 1, 1, new List<StatusInstance>() { new StatusInstance(new Feather(), 5) });
            var player = new Player(hp: 1, maxEnergy: 1, drawAmount: 1);
            var fs = new FightSimulator(cis, enemy, player, false);
            var root = fs.Sim();
            //there should be two randomChoice nodes
            Assert.AreEqual(0, root.Choices.Count);
            Assert.AreEqual(2, root.Randoms.Count);

            foreach (var r in root.Randoms)
            {
                //endturn and play your single card.
                Assert.AreEqual(2, r.Choices.Count);
                Assert.AreEqual(0, r.Randoms.Count);
            }

            //draw orders:
            // S* -win
            // DS* win
            // DD* lose
            //so Value should be 75 and the tree should be exhausted.

            // S win
            // D
            //  S  win
            //  D  lose
        }
    }
}