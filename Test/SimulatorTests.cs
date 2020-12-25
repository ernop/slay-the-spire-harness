
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using static StS.Helpers;

namespace StS.Tests
{
    /// <summary>
    /// These are exhaustive tests. They should go til every option has been explored.
    /// But how can nodes know when the full space has been gone through?
    /// </summary>
    public class McTests
    {
        [SetUp]
        public void Setup()
        {
            SetRandom(0);
        }

        [Test]
        public void Test_Basic()
        {
            //I want to validate that I am properly generating all outcomes.
            var cards = Gsl("Strike");

            var enemy = new Cultist(1);
            var player = new Player(hp: 1);
            var deck = new Deck(cards, Gsl(), Gsl(), Gsl());
            var fs = new MonteCarlo(deck, enemy, player, firstHand: cards);
            var results = fs.SimAfterFirstDraw();
            Assert.AreEqual(1, results.Value.Value);
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
            //It is now a requirement that all fights end before maxTurns
            //since we can't calculate value for incomplete fights.
            var cards = Gsl("Defend", "Defend", "Strike");

            var enemy = new GenericEnemy(amount: 5, count: 1, hp: 8, statuses: GSS(new Feather(), 5));
            var player = new Player(hp: 5); //fullblock first turn. 2nd turn take 5
            var deck = new Deck(cards, Gsl(), Gsl(), Gsl());
            var fs = new MonteCarlo(deck, enemy, player, firstHand: cards);
            var node = fs.SimAfterFirstDraw();
            var best = GetBestLeaf(node);
            Assert.AreEqual(5, node.GetValue().Value);
        }

        [Test]
        public void Test_UsingEnlightenment_ToFindPath()
        {
            var cards = Gsl("Strike", "Strike", "HeavyBlade", "Enlightenment+", "Bash");

            var enemy = new Cultist(hp: 80, hpMax: 80);
            var player = new Player(hp: 1); //player must kill turn 2.

            //best line: play e+, bash (8+vuln), hb (21), strike (9) = 38.
            //turn 2: redraw same cards but play differently.  HB (21) S(9) b(12) = 42
            var deck = new Deck(cards, Gsl(), Gsl(), Gsl());
            var fs = new MonteCarlo(deck, enemy, player, firstHand: cards);
            var node = fs.SimAfterFirstDraw();

            Assert.AreEqual(1, node.GetValue().Value);
            //assert the right cards were played.
            var drawNode = node.Randoms.First();
            var enlightenment = drawNode.Choices.Single(el => el.Value.Value == 1);
            Assert.AreEqual(nameof(Enlightenment), enlightenment.FightAction.Card.Card.Name);
            Assert.AreEqual(1, enlightenment.FightAction.Card.UpgradeCount);

            var bash = enlightenment.Choices.Single(el => el.Value.Value == 1);
            Assert.AreEqual(nameof(Bash), bash.FightAction.Card.Card.Name);
            Assert.AreEqual(0, bash.FightAction.Card.UpgradeCount);
        }

        [Test]
        public void Test_UsingPotionAndInflameDefending()
        {
            var cards = Gsl("Defend", "Defend", "Inflame", "Defend", "Strike");

            var enemy = new GenericEnemy(15, 3, 10, 10);
            var player = new Player(hp: 1, potions: new List<Potion>() { new StrengthPotion() });
            var deck = new Deck(cards, Gsl(), Gsl(), Gsl());
            var fs = new MonteCarlo(deck, enemy, player, firstHand: cards);
            var node = fs.SimAfterFirstDraw();

            Assert.AreEqual(1, node.GetValue().Value);
            //assert the right cards were played.
            var drawNode = node.Randoms.First();
            var wins = drawNode.Choices.Where(el => el.Value.Value == 1);
            Assert.AreEqual(3, wins.Count());
            //block, pot, inflame

        }


        [Test]
        public void Test_PartialDefending()
        {
            //var cis = GetCis("Defend", "Defend", "Defend", "Defend", "Strike");

            //var enemy = new GenericEnemy(amount: 11, count: 1,  hp: 8, hpMax: 8);
            //var player = new Player();
            //var fs = new FightSimulator(cis, enemy, player);
            //var node = fs.Sim();

            //Assert.AreEqual(99, node.GetValue().Value);

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
            var cards = Gsl("Strike", "Defend", "Inflame", "Defend", "Defend");

            var enemy = new GenericEnemy(4, 4, 8, 8);
            var player = new Player(hp: 1);
            var deck = new Deck(cards, Gsl(), Gsl(), Gsl());
            var fs = new MonteCarlo(deck, enemy, player, firstHand: cards);
            var node = fs.SimAfterFirstDraw();

            Assert.AreEqual(1, node.GetValue().Value);
        }

        [Test]
        public void Test_Finds_Single_Line()
        {
            var cards = Gsl("Bash", "Strike");
            var enemy = new GenericEnemy(100, 100, 32, 32);
            var player = new Player(potions: new List<Potion>() { new StrengthPotion(), new StrengthPotion(), new StrengthPotion() }, relics: new List<Relic>() { new FusionHammer() });
            var deck = new Deck(cards, Gsl(), Gsl(), Gsl());
            var fs = new MonteCarlo(deck, enemy, player, firstHand: cards);
            var node = fs.SimAfterFirstDraw();
            Assert.AreEqual(1, node.Randoms.Count);
            //also assert there is only one good path.
            var wins = node.Randoms.First().Choices.Where(el => el.GetValue().Value == 100);
            var win = wins.First();
            var hh = win.GetTurnHistory();
            Assert.AreEqual(1, wins.ToList().Count());

            //draw, sss, bash+strike to just kill
            Assert.AreEqual(5, hh.Count());
            Assert.AreEqual(100, node.GetValue().Value);

            var expectedActions = new List<FightAction>()
            {
                new FightAction(FightActionEnum.Potion, potion: new StrengthPotion(), target:enemy),
                new FightAction(FightActionEnum.Potion, potion: new StrengthPotion(), target:enemy),
                new FightAction(FightActionEnum.Potion, potion: new StrengthPotion(), target:enemy),
                new FightAction(FightActionEnum.PlayCard, card: GetCi("Bash")),
                new FightAction(FightActionEnum.PlayCard, card: GetCi("Strike")),
        };

            Assert.IsTrue(TestActionSeries(win, expectedActions, out var msg), msg);
        }

        public bool TestActionSeries(FightNode root, List<FightAction> actions, out string res)
        {
            var target = root;
            var ii = 0;
            res = "";
            while (ii < actions.Count())
            {
                if (!target.FightAction.IsEqual(actions[ii]))
                {
                    res = $"Action action: {target.FightAction} did not match expected: {actions[ii]}";
                    return false;
                }
                target = target.Value.BestChoice;
                ii++;
            }
            return true;
        }

        [Test]
        public void Test_Fails_Impossible_Fight_Barely()
        {
            var cards = Gsl("Bash", "Inflame", "Strike");
            var enemy = new GenericEnemy(100, 100, 33, 33);
            var player = new Player(potions: new List<Potion>() { new StrengthPotion(), new StrengthPotion() }, 
                relics: new List<Relic>() { new FusionHammer() });
            var deck = new Deck(cards, Gsl(), Gsl(), Gsl());
            var fs = new MonteCarlo(deck, enemy, player, firstHand: cards);
            var node = fs.SimAfterFirstDraw();
            Assert.AreEqual(1, node.Randoms.Count);
            //also assert there is only one good path.
            var best = node.Randoms.Max(el => el.GetValue());
            //var bb = GetBestLeaf(node.Randoms);
            //var hh = bb.AALeafHistory();
            Assert.AreEqual(-1, best.Value);
            Assert.AreEqual(-1, node.GetValue().Value);
        }

        [Test]
        public void Test_Longer_Setup()
        {
            //Correct strategy is to take damage the first round.
            var cis = GetCis("Defend", "Strike", "Defend", "Inflame", /* AFTER */ "Inflame", "Inflame", "Inflame", "Defend");

            var enemyStatuses = GSS(new Feather(), 10);
            enemyStatuses.AddRange(GSS(new Strength(), -10));
            var enemy = new GenericEnemy(amount: 1, count: 5, hp: 14, hpMax: 14, statuses: enemyStatuses);
            var player = new Player(drawAmount: 4, hp: 10);
            var firstHand = new List<string>() { "Inflame", "Inflame", "Inflame", "Defend" };
            var deck = new Deck(cis);

            var fs = new MonteCarlo(deck, enemy, player, firstHand: firstHand);
            var node = fs.SimAfterFirstDraw();
            Assert.AreEqual(1, node.Randoms.Count);
            //also assert there is only one good path.
            var best = node.Randoms.Max(el => el.GetValue());

            var bests = node.Randoms.Where(el => el.GetValue().Value == 5);
            //this is good but there can still be dumb paths that are longer.

            var win = bests.First();
            var hh = win.GetTurnHistory();
            var winNode = GetBestLeaf(node);
            Assert.AreEqual(5, hh.Count());

            Assert.AreEqual(5, best.Value);
            Assert.AreEqual(5, node.GetValue().Value);
        }

        [Test]
        public void Test_SelfControl_SavingPummelAndDefending()
        {
            var draw = Gsl("Pummel", "Inflame", "Inflame",    /* first round cards: */ "Defend", "Defend", "Inflame", "Inflame", "Inflame");
            // correct strat: take 5 first round, playing all inflames then pummel.
            var enemyStatuses = GSS(new Feather(), 10);
            enemyStatuses.AddRange(GSS(new Strength(), -10));
            var enemy = new GenericEnemy(amount: 1, count: 5, hp: 48, statuses: enemyStatuses);
            //after 2nd round enemy will kill player.
            var player = new Player(hp: 10);
            var deck = new Deck(draw, Gsl(), Gsl(), Gsl());
            var firstHand = Gsl("Defend", "Defend", "Inflame", "Inflame", "Inflame");
            var fs = new MonteCarlo(deck, enemy, player, firstHand: firstHand);
            var node = fs.SimAfterFirstDraw();
            Assert.AreEqual(1, node.Randoms.Count);
            //also assert there is only one good path.
            var best = node.Randoms.Max(el => el.GetValue());

            //var bests = node.Randoms.Where(el => el.GetValue().Value == 5);
            //this is good but there can still be dumb paths that are longer.

            //var bf = bests.First();
            //var wn = GetBestLeaf(bf);
            //var hh = wn.AALeafHistory();

            Assert.AreEqual(5, best.Value);
            Assert.AreEqual(5, node.GetValue().Value);
            //node.Display(Output, true);
            //Assert.IsFalse(true);

            var winNode = GetBestLeaf(node);
            var d = winNode.Depth;
            var h = winNode.GetTurnHistory();
            var hh = winNode.AALeafHistory();
            Assert.AreEqual(5, h.Count()); //draw d endturn monsterend start i p

            //this makes sure it doesn't spuriously play a defend first in the last turn.
        }

        [Test]
        public void Test_NodeValue()
        {
            var av = new NodeValue(4, 4, null);
            var av2 = new NodeValue(4, 4, null);
            var bv = new NodeValue(6, 4, null);
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
            //You have a 1/3 chance of living
            var cards = Gsl("Strike", "Defend", "Inflame");

            var enemy = new GenericEnemy(100, 100, 1, 1);
            var player = new Player(drawAmount: 1);
            var deck = new Deck(cards, Gsl(), Gsl(), Gsl());
            var fs = new MonteCarlo(deck, enemy, player);
            var node = fs.SimIncludingDraw();
            Assert.AreEqual(3, node.Randoms.Count);
            var values = node.Randoms.Select(el => el.GetValue().Value).OrderBy(el => el).ToList();
            CollectionAssert.AreEqual(new List<double>() { 100, -1, -1 }.OrderBy(el => el), values);
        }

        [Test]
        public void Test_ExploringDrawSpace()
        {
            var cards = Gsl("Strike", "Defend");
            var enemy = new Cultist(hp: 1, hpMax: 1);
            var player = new Player(hp: 1, maxEnergy: 1, drawAmount: 1);
            var deck = new Deck(cards, Gsl(), Gsl(), Gsl());
            var fs = new MonteCarlo(deck, enemy, player);
            var root = fs.SimIncludingDraw();
            //there should be two randomChoice nodes
            Assert.AreEqual(0, root.Choices.Count);
            Assert.AreEqual(2, root.Randoms.Count);

            foreach (var r in root.Randoms)
            {
                //var v = r.GetValue();
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