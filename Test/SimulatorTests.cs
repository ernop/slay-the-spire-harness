
using System.Collections.Generic;
using System.Linq;

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

            Assert.AreEqual(100, node.GetValue().Value);

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

            Assert.AreEqual(100, node.GetValue().Value);
        }


        [Test]
        public void Test_PartialDefending()
        {
            var cis = GetCis("Defend", "Defend", "Defend", "Defend", "Strike");

            var enemy = new GenericEnemy(11, 1, 8, 8);
            var player = new Player();
            var fs = new FightSimulator(cis, enemy, player);
            var node = fs.Sim();

            Assert.AreEqual(99, node.GetValue().Value);

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

            Assert.AreEqual(100, node.GetValue().Value);
        }

        [Test]
        public void Test_Finds_Single_Line()
        {
            var cis = GetCis("Bash", "Strike");
            var enemy = new GenericEnemy(100, 100, 32, 32);
            var player = new Player(potions: new List<Potion>() { new StrengthPotion(), new StrengthPotion(), new StrengthPotion() }, relics: new List<Relic>() { new FusionHammer() });
            var fs = new FightSimulator(cis, enemy, player);
            var node = fs.Sim();
            Assert.AreEqual(1, node.Randoms.Count);
            //also assert there is only one good path.
            var wins = node.Randoms.Where(el => el.GetValue().Value == 100);
            Assert.AreEqual(1, wins.ToList().Count());
            Assert.AreEqual(100, node.GetValue().Value);
        }

        [Test]
        public void Test_Fails_Impossible_Fight_Barely()
        {
            var cis = GetCis("Bash", "Inflame", "Strike");
            var enemy = new GenericEnemy(100, 100, 33, 33);
            var player = new Player(potions: new List<Potion>() { new StrengthPotion(), new StrengthPotion() }, relics: new List<Relic>() { new FusionHammer() });
            var fs = new FightSimulator(cis, enemy, player);
            var node = fs.Sim();
            Assert.AreEqual(1, node.Randoms.Count);
            //also assert there is only one good path.
            var best = node.Randoms.Max(el => el.GetValue());
            Assert.AreEqual(-1, best.Value);
            Assert.AreEqual(-1, node.GetValue().Value);
        }

        [Test]
        public void Test_Longer_Setup()
        {
            //Correct strategy is to take damage the first round.
            var cis = GetCis("Defend", "Strike", "Defend", "Inflame", /* AFTER */ "Inflame", "Inflame", "Inflame", "Defend");
            var enemy = new GenericEnemy(amount: 1, count: 5, hpMax: 14, hp: 14, statuses: GetStatuses(new Feather(), 10));
            var player = new Player(drawAmount: 4, hp: 10);
            var fs = new FightSimulator(cis, enemy, player, oneStartingHandOnly: true);
            var node = fs.Sim();
            Assert.AreEqual(1, node.Randoms.Count);
            //also assert there is only one good path.
            var best = node.Randoms.Max(el => el.GetValue());

            var bests = node.Randoms.Where(el => el.GetValue().Value == 5);
            //this is good but there can still be dumb paths that are longer.

            Assert.AreEqual(5, best.Value);
            Assert.AreEqual(5, node.GetValue().Value);
            node.Display(Output, true);
            //Assert.IsFalse(true);

            var winNode = GetBestLeaf(node.Randoms.First());
            var d = winNode.Depth;
            Assert.AreEqual(10, d); //draw i i i endturn monsterend start i s + outers

            Assert.AreEqual(FightStatus.Won, winNode.Fight.Status);
            Assert.AreEqual(FightActionEnum.PlayCard, winNode.FightHistory.FightActionType);
            //this makes sure it doesn't spuriously play a defend first in the last turn.
        }

        [Test]
        public void Test_SelfControl_SavingPummelAndDefending()
        {
            //Correct strategy is to take damage the first round.
            var cis = GetCis("Defend", "Inflame", "Defend", "Inflame", "Pummel");
            var enemy = new GenericEnemy(amount: 1, count: 5, hp: 16, statuses: GetStatuses(new Feather(), 10));
            var player = new Player(hp: 10);
            var fs = new FightSimulator(cis, enemy, player, oneStartingHandOnly: true);
            var node = fs.Sim();
            Assert.AreEqual(1, node.Randoms.Count);
            //also assert there is only one good path.
            var best = node.Randoms.Max(el => el.GetValue());

            var bests = node.Randoms.Where(el => el.GetValue().Value == 5);
            //this is good but there can still be dumb paths that are longer.

            Assert.AreEqual(10, best.Value);
            Assert.AreEqual(10, node.GetValue().Value);
            node.Display(Output, true);
            //Assert.IsFalse(true);

            var winNode = GetBestLeaf(node.Randoms.First());
            var d = winNode.Depth;
            Assert.AreEqual(8, d); //draw d endturn monsterend start i p

            Assert.AreEqual(FightStatus.Won, winNode.Fight.Status);
            Assert.AreEqual(FightActionEnum.PlayCard, winNode.FightHistory.FightActionType);
            //this makes sure it doesn't spuriously play a defend first in the last turn.
        }

        public FightNode GetBestLeaf(FightNode f)
        {
            var res = f;
            while (true)
            {
                var bc = res.BestChild();
                if (bc == null)
                {
                    break;
                }
                res = bc;
            }
            return res;
        }

        [Test]
        public void Test_NodeValue()
        {
            var av = new NodeValue(4, 4);
            var av2 = new NodeValue(4, 4);
            var bv = new NodeValue(6, 4);
            Assert.IsTrue(bv > av);
            Assert.IsFalse(bv < av);
            Assert.IsTrue(av == av2);
            Assert.IsTrue(av == av);
            Assert.IsFalse(av != av);
            Assert.IsTrue(av < bv);
            Assert.IsFalse(av > bv);
        }

        [Test]
        public void Test_Fractions()
        {
            var cis = GetCis("Strike", "Defend", "Inflame");
            var enemy = new GenericEnemy(100, 100, 1, 1);
            var player = new Player(drawAmount: 1);
            var fs = new FightSimulator(cis, enemy, player);
            var node = fs.Sim();
            Assert.AreEqual(3, node.Randoms.Count);
            CollectionAssert.AreEqual(new HashSet<double>() { 100, -1, -1 }, node.Randoms.Select(el => el.GetValue().Value).ToHashSet());
        }

        [Test]
        public void Test_ExploringDrawSpace()
        {
            var cis = GetCis("Strike", "Defend");
            var enemy = new GenericEnemy(0, 1, 1, 1, new List<StatusInstance>() { new StatusInstance(new Feather(), 5) });
            var player = new Player(hp: 1, maxEnergy: 1, drawAmount: 1);
            var fs = new FightSimulator(cis, enemy, player, false);
            var root = fs.Sim();
            //there should be two randomChoice nodes
            Assert.AreEqual(0, root.Choices.Count);
            Assert.AreEqual(2, root.Randoms.Count);

            foreach (var r in root.Randoms)
            {
                var v = r.GetValue(true);
                //endturn and play your single card.
                Assert.AreEqual(2, r.Choices.Count);
                Assert.AreEqual(0, r.Randoms.Count);

                //player can always win the fight.
                Assert.AreEqual(1, r.GetValue().Value);
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