using System;
using System.Collections.Generic;

using NUnit.Framework;

using static StS.AllCards;
using static StS.AllRelics;
using static StS.Helpers;

namespace StS.Tests
{
    public class Tests
    {

        public static Dictionary<string, Card> Cards;
        public static Dictionary<string, Relic> Relics;

        [SetUp]
        public void Setup()
        {
            Cards = GetAllCards();
            Relics = GetAllRelics();
        }




        public static void RunTest(string name, int pl = 50, int pl2 = 50, int en = 50, int en2 = 50,
           int plbl = 0, int enbl = 0, int finalPlayerBlock = 0, int finalEnemyBlock = 0,
           List<Relic> relics = null, List<CardInstance> cis = null, EnemyAction ea = null,
           List<StatusInstance> playerStatuses = null, List<StatusInstance> enemyStatuses = null,
           List<StatusInstance> playerFinalStatuses = null,
           List<StatusInstance> enemyFinalStatuses = null,
           int? playerEnergy = null,
           int? finalEnergy = null,
           int? expectedExhausePileCount = null)
        {
            Console.WriteLine($"====Testcase {name}");
            if (cis == null)
            {
                cis = new List<CardInstance>();
            }

            var gc = new GameContext();
            var player = new Player(hpMax: pl, hp: pl);
            if (relics != null)
            {
                player.Relics = relics;
                foreach (var relic in player.Relics)
                {
                    relic.Player = player;
                }
            }



            gc.Player = player;


            var enemies = SetupEnemies("Name", en, enbl, enemyStatuses);

            var enemy = enemies[0];

            var fight = new Fight(cis, gc, player, enemies);

            //todo player.GetDrawAmount()
            fight.FirstTurnStarts(5);

            //these apply after the fight started. conceptually having tests that set up artificial situations is going to cause lots of problems.
            player.StatusInstances = playerStatuses ?? new List<StatusInstance>();
            player.Block = plbl;


            player.Energy = playerEnergy ?? player.Energy;


            Console.WriteLine($"Enemy: {enemy}");
            Console.WriteLine($"Player: {player}");

            foreach (var ci in cis)
            {
                fight.PlayCard(ci, player, enemy);

                Console.WriteLine($"Player:{player}");
                Console.WriteLine($"Enemy:{enemy}");
            }

            //For now no targeting for enemy cards.
            fight.ApplyEnemyAction(ea, enemy, player);

            Console.WriteLine($"Player:{player}");
            Console.WriteLine($"Enemy:{enemy}");


            if (Helpers.PrintDetails)
            {
                Console.WriteLine("\tFinal State:");
                Console.WriteLine("\t" + player);
                Console.WriteLine("\t" + enemy);
            }
            if (player.HP != pl2)
            {
                throw new Exception($"{name} Player hp={player.HP} expected to be={pl2}");
            }
            if (enemy.HP != en2)
            {
                throw new Exception($"{name} Enemy hp={enemy.HP} expected to be={en2}");
            }
            if (player.Block != finalPlayerBlock)
            {
                throw new Exception($"{name} PlayerBlock expected:{finalPlayerBlock} actual:{player.Block}");
            }
            if (enemy.Block != finalEnemyBlock)
            {
                throw new Exception($"{name} EnemyBlock expected:{finalEnemyBlock} actual:{enemy.Block}");
            }
            if (playerFinalStatuses?.Count > 0)
            {
                if (!CompareStatuses(playerFinalStatuses, player.StatusInstances, out var error))
                {
                    throw new Exception($"bad statuses. {error}");
                }
            }
            if (enemyFinalStatuses?.Count > 0)
            {
                if (!CompareStatuses(enemyFinalStatuses, enemy.StatusInstances, out var error))
                {
                    throw new Exception($"bad statuses. {error}");
                }
            }

            if (finalEnergy.HasValue)
            {
                if (player.Energy != finalEnergy.Value)
                {
                    throw new Exception($"Expected energy: {finalEnergy.Value} actual: {player.Energy}");
                }
            }

            if (expectedExhausePileCount.HasValue)
            {
                var ex = fight.GetExhaustPile();
                if (ex.Count != expectedExhausePileCount.Value)
                {
                    throw new Exception($"expected exhaust pile count: {expectedExhausePileCount.Value} actual {ex.Count}");
                }
            }


            Console.WriteLine($"====Testcase {name} is valid\n");

        }


        [Test]
        public static void BasicTests()
        {
            var statuses = new List<StatusInstance>() { new StatusInstance(new Vulnerable(), 8), new StatusInstance(new Weak(), 8) };
            RunTest(name: "Shockwave+VulnWeakAttack", en2: 37, pl2: 26, cis: GetCis("Shockwave", "Shockwave+", "Strike+"), enemyFinalStatuses: statuses, ea: Attack(8, 4), playerEnergy: 10);
            RunTest(name: "BashIronwave", en2: 32, cis: GetCis("Bash", "IronWave+"), finalPlayerBlock: 7);
            RunTest(name: "BashInflameIronwave", en2: 27, cis: GetCis("Bash", "Inflame+", "IronWave+"), finalPlayerBlock: 7, playerEnergy: 10);

            RunTest(name: "DefendNeg3", cis: GetCis("Footwork", "Defend"), finalPlayerBlock: 2, playerStatuses: new List<StatusInstance>() { new StatusInstance(new Dexterity(), -5) });
            RunTest(name: "DefendNeg5", cis: GetCis("Footwork", "Defend"), finalPlayerBlock: 0, playerStatuses: new List<StatusInstance>() { new StatusInstance(new Dexterity(), -7) });
            RunTest(name: "DefendNeg2", cis: GetCis("Footwork", "Defend"), finalPlayerBlock: 3, playerStatuses: new List<StatusInstance>() { new StatusInstance(new Dexterity(), -4) });
            RunTest(name: "Defend", cis: GetCis("Defend"), finalPlayerBlock: 5);
            RunTest(name: "FootDefend", cis: GetCis("Footwork", "Defend"), finalPlayerBlock: 7);
            RunTest(name: "Foot+Defend", cis: GetCis("Footwork+", "Defend"), finalPlayerBlock: 8);
            RunTest(name: "FootFoot+Defend", cis: GetCis("Footwork", "Footwork+", "Defend"), finalPlayerBlock: 10);
            RunTest(name: "Strike works", en2: 44, cis: GetCis("Strike"));
            RunTest(name: "Strike, Strike+ works", en2: 35, cis: GetCis("Strike", "Strike+"));


            RunTest(name: "Bashing", en2: 19, cis: GetCis("Strike", "Bash+", "Strike+"), playerEnergy: 10);
            RunTest(name: "SwordBoomerang", en2: 41, cis: GetCis("SwordBoomerang"));
            RunTest(name: "SwordBoomerang+", en2: 38, cis: GetCis("SwordBoomerang+"));
            RunTest(name: "SwordBoomerang+ vs block1", en2: 39, cis: GetCis("SwordBoomerang+"), enbl: 1, finalEnemyBlock: 0);
            RunTest(name: "SwordBoomerang+ vs block2", en2: 40, cis: GetCis("SwordBoomerang+"), enbl: 2, finalEnemyBlock: 0);
            RunTest(name: "SwordBoomerang+ vs block3", en2: 41, cis: GetCis("SwordBoomerang+"), enbl: 3, finalEnemyBlock: 0);
            RunTest(name: "SwordBoomerang+ vs block4", en2: 42, cis: GetCis("SwordBoomerang+"), enbl: 4, finalEnemyBlock: 0);
            RunTest(name: "SwordBoomerang+ vs block13", cis: GetCis("SwordBoomerang+"), enbl: 13, finalEnemyBlock: 1);
            RunTest(name: "Inflamed SwordBoomerang+", en2: 30, cis: GetCis("Inflame", "SwordBoomerang+"));

            RunTest(name: "Inflame", en2: 35, cis: GetCis("Strike", "Inflame+", "Strike"));
            RunTest(name: "Inflame+Bash", en2: 39, cis: GetCis("Inflame+", "Bash"));
            RunTest(name: "Inflame+Bash+strike", en2: 26, cis: GetCis("Inflame+", "Bash", "Strike"), playerEnergy: 10);
            RunTest(name: "Inflame+LimitBreak", en2: 32, cis: GetCis("Strike", "Inflame+", "LimitBreak", "Strike"), playerEnergy: 10);
            RunTest(name: "Inflame+LimitBreak+bash", en2: 8, cis: GetCis("Strike", "Inflame+", "LimitBreak", "Bash", "Strike+"), playerEnergy: 10);

            RunTest(name: "WeakPlayer", cis: GetCis("Strike"), en2: 46, playerStatuses: new List<StatusInstance>() { new StatusInstance(new Weak(), 3) });
            RunTest(name: "WeakenEnemy", cis: GetCis("Uppercut+"), en2: 37,
                enemyFinalStatuses: new List<StatusInstance>() { new StatusInstance(new Vulnerable(), 2), new StatusInstance(new Weak(), 2) });

            RunTest(name: "Entrench", cis: GetCis("Footwork+", "Defend+", "Entrench", "Defend"), finalPlayerBlock: 30, playerEnergy: 10);
        }

        [Test]
        public static void RelicTests()
        {
            var vajraOne = new Vajra { Intensity = 1 };
            RunTest(name: "Strike, Strike+ works", en2: 33, cis: GetCis("Strike", "Strike+"),
                relics: new List<Relic>() { vajraOne });

            var torii = new Torii();
            RunTest(name: "Torii works", pl2: 47,
                relics: new List<Relic>() { torii },
                ea: Attack(5, 3));

            RunTest(name: "Torii Strong Enemy", pl2: 10,
                relics: new List<Relic>() { torii },
                enemyStatuses: new List<StatusInstance>() { new StatusInstance(new Strength(), 4) },
                ea: Attack(6, 4));

            RunTest(name: "Torii Strong Enemy Weakest attakc", pl2: 46,
                relics: new List<Relic>() { torii },
                enemyStatuses: new List<StatusInstance>() { new StatusInstance(new Strength(), 4) },
                ea: Attack(1, 4));

            TestMonkeyPawInflames();
            TestMonkeyPaw();

            var penNib = new PenNib();
            penNib.AttackCount = 8;
            RunTest(name: "PenNib toggles-untoggles", en2: 20, cis: GetCis("Strike", "Strike+", "Strike"), relics: new List<Relic>() { penNib });

            var penNib2 = new PenNib();
            penNib2.AttackCount = 2;
            RunTest(name: "PenNib-Nonfunctional", en2: 35, cis: GetCis("Strike", "Strike+"), relics: new List<Relic>() { penNib2 });

            var penNib3 = new PenNib();
            penNib3.AttackCount = 8;
            RunTest(name: "PenNib-Inflame", en2: 20, cis: GetCis("Strike", "Inflame+", "Strike+"), relics: new List<Relic>() { penNib3 });

            var penNib4 = new PenNib();
            penNib4.AttackCount = 8;
            RunTest(name: "Inflamed Nibbed SwordBoomerang+", en2: 4, cis: GetCis("Strike", "Inflame", "SwordBoomerang+"), relics: new List<Relic>() { penNib4 });
        }

        [Test]
        public static void EnemyBehaviorTests()
        {
            var si = new List<StatusInstance>() { new StatusInstance(new Aggressive(), 4) };
            RunTest(name: "Louse-Aggressive", en2: 41, cis: GetCis("Strike+"), enemyStatuses: si, finalEnemyBlock: 4);

            RunTest(name: "Enemy-attacks-disarmed", pl2: 43, en2: 41, cis: GetCis("Strike+", "Disarm+"), ea: Attack(10, 1));

            RunTest(name: "Enemy-attacks-bash", pl2: 20, en2: 41, cis: GetCis("Strike+"), ea: Attack(10, 2),
                playerStatuses: new List<StatusInstance>() { new StatusInstance(new Vulnerable(), 3) });

            var si2 = new List<StatusInstance>() { new StatusInstance(new Aggressive(), 4) };
            RunTest(name: "Louse-Aggressive-triggered-cleared", en2: 33, cis: GetCis("Strike+", "Inflame+", "LimitBreak", "Strike"), enemyStatuses: si2, finalEnemyBlock: 0, playerEnergy: 10);

            RunTest(name: "Enemy-attacks", pl2: 40, en2: 41, cis: GetCis("Strike+"), ea: Attack(10, 1));
        }

        [Test]
        public static void TestHeadbutt()
        {
            Console.WriteLine($"Starting{nameof(TestHeadbutt)}");
            var player = new Player();
            var gc = new GameContext();
            var enemy = new Enemy();
            var initialCis = GetCis("Strike+", "Bash+", "Headbutt");
            var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
            fight.FirstTurnStarts(2);

            var hand = fight.GetHand();

            //problem: when I initialize the fight I make a copy of the cards.
            fight.PlayCard(initialCis[1], player, enemy);
            fight.PlayCard(initialCis[2], player, enemy, new List<CardInstance>() { initialCis[1] });
            //now ensure that headbutt is at the end of the draw pile.

            var dp = fight.GetDrawPile();
            if (dp.Count != 2)
            {
                throw new Exception($"{nameof(TestHeadbutt)}: bad draw pile");
            }

            if (dp[dp.Count - 1] != initialCis[1])
            {
                throw new Exception($"{nameof(TestHeadbutt)}: headbutt didn't work");
            }
            Console.WriteLine($"{nameof(TestHeadbutt)} works.");
        }

        [Test]
        public static void TestMonkeyPaw()
        {
            Console.WriteLine($"Starting{nameof(TestMonkeyPaw)}");
            var player = new Player();
            player.Relics.Add(Relics["MonkeyPaw"]);
            var gc = new GameContext();
            var enemy = new Enemy();
            var initialCis = GetCis("Inflame+", "Bash+");
            var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
            fight.FirstTurnStarts(5);

            //problem: when I initialize the fight I make a copy of the cards.
            fight.PlayCard(initialCis[0], player, enemy);
            var hand = fight.GetHand();
            var card = hand[0];
            var ec = card.EnergyCost();
            if (ec != 0)
            {
                throw new Exception("Monkey paw didn't work");
            }
            fight.PlayCard(card, player, enemy);
            fight.NextTurn(5);

            var secondHand = fight.GetHand();
            if (!CompareHands(secondHand, GetCis("Bash+"), out string message))
            {
                throw new Exception($"Invalid hand; bash should have had cost zero. {message}");
            }
            Console.WriteLine("==========");
        }

        [Test]
        public static void Test_Statuses()
        {
            RunTest("Slimed", cis: GetCis("Slimed", "Slimed", "Slimed"), finalEnergy: 0);
        }

        [Test]
        public static void Test_Dazed()
        {
            var player = new Player();
            var gc = new GameContext();
            var enemy = new Enemy();
            var initialCis = GetCis("Dazed", "Dazed", "Slimed");
            var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
            fight.FirstTurnStarts();
            fight.NextTurn();
            var exhaust = fight.GetExhaustPile();
            Assert.AreEqual(exhaust.Count, 2);

            fight.PlayCard(initialCis[2], player, enemy);
            fight.NextTurn();
            var hand = fight.GetHand();
            Assert.AreEqual(hand.Count, 0, "Card should have exhausted");

            var exhaust2 = fight.GetExhaustPile();
            Assert.AreEqual(exhaust2.Count, 3);
        }

        [Test]
        public static void Test_Shockwave()
        {
            var player = new Player();
            var gc = new GameContext();
            var enemy = new Enemy();
            var initialCis = GetCis("Shockwave+");
            var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
            fight.FirstTurnStarts();

            fight.PlayCard(initialCis[0], player, enemy);
            fight.NextTurn();
            fight.NextTurn();
            var hand = fight.GetHand();
            Assert.AreEqual(hand.Count, 0, "Card should have exhausted");

            var exhaust = fight.GetExhaustPile();
            Assert.AreEqual(exhaust.Count, 1, "Shockwave should have exhausted failed.");

            Console.WriteLine("==========");
        }

        [Test]
        public static void Test_Shockwave_NotEthereal()
        {
            var player = new Player();
            var gc = new GameContext();
            var enemy = new Enemy();
            var initialCis = GetCis("Shockwave+");
            var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
            fight.FirstTurnStarts();

            fight.NextTurn();
            fight.NextTurn();
            var hand = fight.GetHand();
            Assert.AreEqual(hand.Count, 1);

            var exhaust = fight.GetExhaustPile();
            Assert.AreEqual(exhaust.Count, 0);

            Console.WriteLine("==========");
        }

        [Test]
        public static void Test_Carnage_Ethereality()
        {
            var player = new Player();
            var gc = new GameContext();
            var enemy = new Enemy();
            var initialCis = GetCis("Carnage+");
            var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
            fight.FirstTurnStarts();

            //problem: when I initialize the fight I make a copy of the cards.

            fight.PlayCard(initialCis[0], player, enemy);
            fight.NextTurn();
            fight.NextTurn();
            var hand = fight.GetHand();
            Assert.AreEqual(hand.Count, 0, "Card should have exhausted due to ethereality");

            var exhaust = fight.GetExhaustPile();
            Assert.AreEqual(exhaust.Count, 1, "Carnage ethereality failed.");
        }

        [Test]
        public static void Test_Carnage_Playable_Without_Exhaustion()
        {
            var player = new Player();
            var gc = new GameContext();
            var enemy = new Enemy();
            var initialCis = GetCis("Carnage+");
            var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
            fight.FirstTurnStarts();

            //problem: when I initialize the fight I make a copy of the cards.

            fight.PlayCard(initialCis[0], player, enemy);
            fight.NextTurn();
            fight.PlayCard(initialCis[0], player, enemy);
            fight.NextTurn();
            var hand = fight.GetHand();
            Assert.AreEqual(hand.Count, 1, "Card should have exhausted due to ethereality");

            var exhaust = fight.GetExhaustPile();
            Assert.AreEqual(exhaust.Count, 0, "Carnage ethereality failed.");
        }

        [Test]
        public static void TestPerfectedStrike()
        {
            Console.WriteLine($"Starting{nameof(TestPerfectedStrike)}");
            var player = new Player();
            var gc = new GameContext();
            var enemy = new Enemy();
            var initialCis = GetCis("Strike+", "PerfectedStrike+", "TwinStrike");
            var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
            fight.FirstTurnStarts(5);

            //problem: when I initialize the fight I make a copy of the cards.

            fight.PlayCard(initialCis[0], player, enemy);

            //now we can play it.
            fight.PlayCard(initialCis[1], player, enemy);

            var expected = 26;
            if (enemy.HP != expected)
            {
                throw new Exception($"{nameof(TestPerfectedStrike)}");
            }
            Console.WriteLine("==========");
        }
        [Test]
        public static void TestClash()
        {
            var player = new Player();
            var gc = new GameContext();
            var enemy = new Enemy();
            var initialCis = GetCis("Inflame+", "Clash");
            var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
            fight.FirstTurnStarts(5);

            //problem: when I initialize the fight I make a copy of the cards.
            bool gotException;
            try
            {
                fight.PlayCard(initialCis[1], player, enemy);
                gotException = false;
            }
            catch
            {
                gotException = true;
            }

            if (!gotException)
            {
                throw new Exception("Allowed playing clash with non-attack in hand.");
            }

            fight.PlayCard(initialCis[0], player, enemy);

            //now we can play it.
            fight.PlayCard(initialCis[1], player, enemy);
        }

        [Test]
        public static void Test_PommelStrike()
        {
            var player = new Player();
            var gc = new GameContext();
            var enemy = new Enemy();
            var initialCis = GetCis("Inflame+", "Strike+", "PommelStrike");
            var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
            fight.FirstTurnStarts(1);

            fight.PlayCard(initialCis[2], player, enemy);
            var hand = fight.GetHand();
            Assert.IsTrue(CompareHands(hand, GetCis("Strike+"), out var message), message);
        }

        [Test]
        public static void Test_PommelStrike_Override()
        {
            var player = new Player();
            var gc = new GameContext();
            var enemy = new Enemy();
            var initialCis = GetCis("Inflame+", "Strike+", "PommelStrike");
            var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
            fight.FirstTurnStarts(1);

            ///force us to draw the inflame
            var targets = new List<CardInstance>() { initialCis[0] };

            fight.PlayCard(initialCis[2], player, enemy, targets);
            var hand = fight.GetHand();
            Assert.IsTrue(CompareHands(hand, GetCis("Inflame+"), out var message), message);
        }

        [Test]
        public static void Test_PommelStrike2()
        {
            var player = new Player();
            var gc = new GameContext();
            var enemy = new Enemy();
            var initialCis = GetCis("Inflame+", "Strike+", "PommelStrike+");
            var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
            fight.FirstTurnStarts(1);

            fight.PlayCard(initialCis[2], player, enemy);
            var hand = fight.GetHand();
            Assert.IsTrue(CompareHands(hand, GetCis("Inflame+", "Strike+"), out var message), message);
        }

        [Test]
        public static void Test_PommelStrike_Rotation()
        {
            var player = new Player();
            var gc = new GameContext();
            var enemy = new Enemy();
            var initialCis = GetCis("Inflame+", "Strike+", "PommelStrike+");
            var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
            fight.FirstTurnStarts(2);

            fight.PlayCard(initialCis[1], player, enemy);
            fight.PlayCard(initialCis[2], player, enemy);
            //hand has rotated.
            var hand = fight.GetHand();
            Assert.IsTrue(CompareHands(hand, GetCis("Inflame+", "Strike+"), out var message), message);
        }


        [Test]
        public static void Test_ShrugItOff1()
        {

            var player = new Player();
            var gc = new GameContext();
            var enemy = new Enemy();
            var initialCis = GetCis("Inflame+", "Strike+", "ShrugItOff");
            var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
            fight.FirstTurnStarts(3);

            //problem: when I initialize the fight I make a copy of the cards.

            fight.PlayCard(initialCis[0], player, enemy);
            fight.PlayCard(initialCis[1], player, enemy);
            fight.PlayCard(initialCis[2], player, enemy);
            var hand = fight.GetHand();
            Assert.IsTrue(CompareHands(hand, GetCis("Strike+"), out var message), message);
        }

        [Test]
        public static void Test_ShrugItOff2()
        {
            var player = new Player();
            var gc = new GameContext();
            var enemy = new Enemy();
            var initialCis = GetCis("FlameBarrier", "Inflame+", "Strike+", "ShrugItOff");
            var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
            fight.FirstTurnStarts(3);

            fight.PlayCard(initialCis[1], player, enemy);
            fight.PlayCard(initialCis[2], player, enemy);
            fight.PlayCard(initialCis[3], player, enemy);
            var hand = fight.GetHand();
            Assert.IsTrue(CompareHands(hand, GetCis("FlameBarrier"), out var message), message);
        }

        [Test]
        public static void Test_BagOfEyes()
        {
            var player = new Player();
            player.Relics.Add(Relics["BagOfEyes"]);
            var gc = new GameContext();
            var enemy = new Enemy();
            var initialCis = GetCis();
            var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
            fight.FirstTurnStarts();
            Assert.True(CompareStatuses(enemy.StatusInstances, new List<StatusInstance>() { new StatusInstance(new Vulnerable(), 1) }, out string error), error);
            fight.NextTurn();
            Assert.True(CompareStatuses(enemy.StatusInstances, new List<StatusInstance>(), out string error2), $"Enemy status not cleared {error2}");
        }

        [Test]
        public static void Test_Anchor()
        {
            var player = new Player();
            player.Relics.Add(Relics["Anchor"]);
            var gc = new GameContext();
            var enemy = new Enemy();
            var initialCis = GetCis();
            var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
            fight.FirstTurnStarts();
            Assert.AreEqual(player.Block, 10);
            fight.NextTurn();
            Assert.AreEqual(player.Block, 0);
        }

        [Test]
        public static void Test_HornCleat()
        {
            var player = new Player();
            player.Relics.Add(Relics["HornCleat"]);
            var gc = new GameContext();
            var enemy = new Enemy();
            var initialCis = GetCis();
            var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
            fight.FirstTurnStarts();
            Assert.AreEqual(player.Block, 0);
            fight.NextTurn();
            Assert.AreEqual(player.Block, 14);
            fight.NextTurn();
            Assert.AreEqual(player.Block, 0);
        }

        [Test]
        public static void TestMonkeyPawInflames()
        {
            var player = new Player();
            player.Relics.Add(Relics["MonkeyPaw"]);
            var gc = new GameContext();
            var enemy = new Enemy();
            var initialCis = GetCis("Inflame+", "Inflame+", "Inflame+", "Inflame+");
            var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
            fight.FirstTurnStarts(5);

            //problem: when I initialize the fight I make a copy of the cards.
            fight.PlayCard(initialCis[0], player, enemy);
            var hand = fight.GetHand();

            while (hand.Count > 0)
            {
                hand = fight.GetHand();
                foreach (var card in hand)
                {
                    if (card.EnergyCost() != 0)
                    {
                        continue;
                    }
                    fight.PlayCard(card, player, enemy);
                    break;
                }
            }

            if (player.Energy != 2)
            {
                throw new Exception("Paw didn't work.");
            }
        }

        [Test]
        public static void DrawTests()
        {
            TestDrawOnly("Basic-Hand-two-from-six", GetCis("Strike+", "Bash", "Inflame", "Shockwave", "LimitBreak", "Footwork"), GetCis("LimitBreak", "Footwork"), 2);
            TestDrawOnly("Basic-Hand-single", GetCis("Strike+"), GetCis("Strike+"), 1);
            TestDrawOnly("Basic-Hand-overdraw", GetCis("Strike+"), GetCis("Strike+"), 2);
            TestDrawOnly("Basic-Hand-draw-one-from-two", GetCis("Strike+", "Bash"), GetCis("Bash"), 1);
            TestDrawOnly("Basic-Hand normal subset", GetCis("Strike+", "Strike", "Bash", "Disarm", "Inflame", "Footwork"), GetCis("Disarm", "Inflame", "Footwork"), 3);
            TestDrawOnly("Basic-Hand normal overdraw", GetCis("Strike+", "Strike", "Bash", "Disarm"), GetCis("Strike+", "Strike", "Bash", "Disarm"), 7);
            TestDrawOnly("Basic-Hand turns.", GetCis("Strike+", "Strike", "Bash", "Disarm"), GetCis("Bash"), 1, 1);
            TestDrawOnly("Basic-Hand turns.", GetCis("Strike+", "Strike", "Bash", "Disarm"), GetCis("Strike+"), 1, 3);
            TestDrawOnly("Powers Disappear", GetCis("Strike+", "Strike", "Bash", "Disarm"), GetCis("Strike+"), 1, 3);

        }

        public static void TestDrawOnly(string testName, List<CardInstance> initialCis, List<CardInstance> expectedCis, int drawCount = 5, int extraTurns = 0, int? energyAfter = null, CharacterType? characterType = CharacterType.IronClad)
        {
            var player = new Player(characterType.Value);
            var gc = new GameContext();
            var enemy = new Enemy();
            var fight = new Fight(initialCis, gc, player, new List<Enemy>() { enemy }, true);

            //initial card draw.
            fight.FirstTurnStarts(drawCount);

            while (extraTurns > 0)
            {
                fight.NextTurn(drawCount);
                extraTurns--;
            }

            var hand = fight.GetHand();
            Assert.IsTrue(CompareHands(hand, expectedCis, out string message));

            if (energyAfter.HasValue)
            {
                Assert.AreEqual(player.Energy, energyAfter, $"Expected energy={energyAfter.Value} actual={player.Energy}");
            }
        }

        [Test]
        public static void DamageBlockTests()
        {
            //TODO this needs fixing.  When the player is weak and target is vuln, do we math.floor both times? or just once at the end.
            RunTest(name: "Weak-Inflame-BodySlam-Vulned", en2: 30, finalPlayerBlock: 10, cis: GetCis("Footwork", "Defend+", "Bash", "Inflame+", "BodySlam+"), playerStatuses: GetStatuses(new Weak(), 2), playerEnergy: 10);

            RunTest(name: "BodySlam", en2: 40, finalPlayerBlock: 10, cis: GetCis("Footwork", "Defend+", "BodySlam+"));
            RunTest(name: "BodySlam-Vulned", en2: 27, finalPlayerBlock: 10, cis: GetCis("Footwork", "Defend+", "Bash", "BodySlam+"), playerEnergy: 10);
            RunTest(name: "Inflame-BodySlam-Vulned", en2: 23, finalPlayerBlock: 10, cis: GetCis("Footwork", "Defend+", "Bash", "Inflame+", "BodySlam+"), playerEnergy: 10);


            RunTest(name: "FlameBarrier", pl2: 50, en2: 34, cis: GetCis("FlameBarrier"), finalPlayerBlock: 8, finalEnemyBlock: 0, ea: Attack(1, 4));
            RunTest(name: "FlameBarrier-player-block1", plbl: 10, pl2: 50, en2: 36, enbl: 10, cis: GetCis("FlameBarrier+"), finalPlayerBlock: 22, finalEnemyBlock: 0, ea: Attack(1, 4));
            RunTest(name: "FlameBarrier-block2", pl2: 50, finalPlayerBlock: 12, en2: 36, enbl: 10, cis: GetCis("FlameBarrier+"), finalEnemyBlock: 0, ea: Attack(1, 4));
            RunTest(name: "FlameBarrier-block3", pl2: 50, enbl: 41, finalPlayerBlock: 24, finalEnemyBlock: 1, cis: GetCis("Inflame+", "Inflame+", "FlameBarrier+", "FlameBarrier"), ea: Attack(1, 4), playerEnergy: 10);
            RunTest(name: "FlameBarrier-block4", pl2: 50, enbl: 39, en2: 49, finalPlayerBlock: 24, finalEnemyBlock: 0, cis: GetCis("Inflame+", "Inflame+", "FlameBarrier+", "FlameBarrier"), ea: Attack(1, 4), playerEnergy: 10);

            RunTest(name: "ClearingEnemyBlock", en2: 34, enbl: 10, cis: GetCis("Strike+", "Bash", "Strike"), finalEnemyBlock: 0, playerEnergy: 10);
            RunTest(name: "FullBlocked", en2: 50, enbl: 26, cis: GetCis("Strike+", "Bash", "Strike"), finalEnemyBlock: 0, playerEnergy: 10);
            RunTest(name: "VulnBreakThrough", en2: 49, enbl: 16, cis: GetCis("Bash", "Strike"), finalEnemyBlock: 0);
            RunTest(name: "HeavyBlade", en2: 27, enbl: 20, cis: GetCis("HeavyBlade", "Inflame+", "HeavyBlade+"), finalEnemyBlock: 0, playerEnergy: 10);

            RunTest(name: "double-FlameBarrier-block", pl2: 50, finalPlayerBlock: 24, enbl: 0, en2: 10,
                cis: GetCis("Inflame+", "Inflame+", "FlameBarrier+", "FlameBarrier"), ea: Attack(1, 4), playerEnergy: 10);
            RunTest(name: "Combining statuses works", finalPlayerBlock: 28, cis: GetCis("Inflame+", "Inflame", "FlameBarrier+", "FlameBarrier"),
                playerFinalStatuses: new List<StatusInstance>() {
                    new StatusInstance(new Strength(), 5),
                    new StatusInstance(new FlameBarrierStatus(), 10)},
                playerEnergy: 10);

            RunTest(name: "BustedCrown-Gives", cis: GetCis(),
                finalEnergy: 4,
                relics: new List<Relic>() { Relics["BustedCrown"] });

            RunTest(name: "Start-3energy", cis: GetCis(),
                finalEnergy: 3,
                relics: new List<Relic>() { Relics["MonkeyPaw"] });

            RunTest(name: "Start-3-paw", cis: GetCis("Inflame", "Strike"),
                finalEnergy: 2,
                relics: new List<Relic>() { Relics["MonkeyPaw"] }, en2: 42);

            RunTest(name: "Start-4-paw", cis: GetCis("Inflame", "Strike"),
                finalEnergy: 3,
                relics: new List<Relic>() { Relics["BustedCrown"], Relics["MonkeyPaw"] }, en2: 42);

            RunTest(name: "Start-4-paw-zero-out-nothing", cis: GetCis("Strike", "Inflame"),
                finalEnergy: 2,
                relics: new List<Relic>() { Relics["BustedCrown"], Relics["MonkeyPaw"] }, en2: 44);

            RunTest(name: "Energy-relics-combine", cis: GetCis("Strike", "Inflame"),
                finalEnergy: 3,
                relics: new List<Relic>() { Relics["FusionHammer"], Relics["BustedCrown"] }, en2: 44);

            RunTest(name: "Energy-relics-combine2", cis: GetCis("Strike", "Inflame"),
                finalEnergy: 3,
                relics: new List<Relic>() { Relics["BustedCrown"], Relics["FusionHammer"] }, en2: 44);

        }

        [Test]
        public static void TestFights()
        {
            var pl = new Player();
            var en = new Cultist();
            var cis = GetCis("Strike+", "Defend+");
            var fight = new Fight(cis, new GameContext(), pl, new List<Enemy>() { en });
            fight.FirstTurnStarts();

            while (true)
            {
                var hand = fight.GetHand().ToArray();
                foreach (var ci in hand)
                {
                    fight.PlayCard(ci, pl, en);
                    if (fight.EnemyDead())
                    {
                        break;
                    }
                }
                var enemyAction = en.GetAction();
                fight.ApplyEnemyAction(enemyAction, en, pl);

                if (fight.Status != Fight.FightStatus.Ongoing)
                {
                    break;
                }

                fight.NextTurn(4);
            }
        }

        public static List<StatusInstance> GetStatuses(Status status, int num)
        {
            return new List<StatusInstance>() { new StatusInstance(status, num) };
        }

        public static EnemyAction Attack(int amount, int count)
        {
            return new EnemyAction(null, new EnemyAttack(amount, count), null);
        }

        public static List<CardInstance> GetCis(params string[] names)
        {
            var cis = new List<CardInstance>();
            foreach (var name in names)
            {
                var x = SplitCardName(name);
                var card = Cards[x.Item1];
                if (card == null)
                {
                    throw new Exception("Missing card.");
                }

                var ci = new CardInstance(card, x.Item2);
                cis.Add(ci);
            }
            return cis;
        }

        public static EnemyAction GetEnemyAction(EnemyAttack card)
        {
            return new EnemyAction(null, card, null);
        }

        public static List<Enemy> SetupEnemies(string en, int ehp, int enbl, List<StatusInstance> enemyStatuses)
        {
            var enemy = new Enemy(en, ehp, ehp);
            enemy.Block = enbl;

            enemy.StatusInstances = enemyStatuses ?? new List<StatusInstance>();

            var enemies = new List<Enemy>() { enemy };
            return enemies;
        }

    }
}