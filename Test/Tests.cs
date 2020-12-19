using System;
using System.Collections.Generic;

using NUnit.Framework;

using static StS.AllRelics;
using static StS.Helpers;

namespace StS.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            Helpers.SetRandom(0);
        }

        public static void RunTest(string name, int pl = 50, int pl2 = 50, int en = 50, int en2 = 50,
           int plbl = 0, int enbl = 0, int finalPlayerBlock = 0, int finalEnemyBlock = 0,
           List<Relic> relics = null, List<CardInstance> cis = null, int? amount = null, int? count = 0,
           List<StatusInstance> playerStatuses = null, List<StatusInstance> enemyStatuses = null,
           List<StatusInstance> playerFinalStatuses = null,
           List<StatusInstance> enemyFinalStatuses = null,
           int? playerEnergy = null,
           int? finalEnergy = null,
           int? expectedExhausePileCount = null)
        {
            if (cis == null)
            {
                cis = new List<CardInstance>();
            }

            var player = new Player(hpMax: pl, hp: pl);
            if (playerStatuses != null)
            {
                player.StatusInstances = playerStatuses;
            }
            if (relics != null)
            {
                player.Relics = relics;
                foreach (var relic in player.Relics)
                {
                    relic.Player = player;
                }
            }

            var enemies = SetupEnemies("Name", en, enbl, enemyStatuses);

            var enemy = enemies[0];

            var fight = new Fight(cis, player, enemy);

            //todo player.GetDrawAmount()
            fight.StartTurn();
            player.Block = plbl;
            player.Energy = playerEnergy ?? player.Energy;

            foreach (var ci in cis)
            {
                fight.PlayCard(ci);
            }

            //For now no targeting for enemy cards.
            if (amount != null) //only if there was an action specified
            {
                fight.EnemyMove(amount.Value, count.Value);
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
                var ex = fight.GetExhaustPile;
                if (ex.Count != expectedExhausePileCount.Value)
                {
                    throw new Exception($"expected exhaust pile count: {expectedExhausePileCount.Value} actual {ex.Count}");
                }
            }
        }



        [Test]
        public static void BasicTests()
        {
            var statuses = new List<StatusInstance>() { new StatusInstance(new Vulnerable(), 7), new StatusInstance(new Weak(), 7) };
            RunTest(name: "Shockwave+VulnWeakAttack", en2: 37, pl2: 26, cis: GetCis("Shockwave", "Shockwave+", "Strike+"), enemyFinalStatuses: statuses, amount: 8, count: 4, playerEnergy: 10);
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


            RunTest(name: "Bashing", en2: 21, cis: GetCis("Strike", "Bash+", "Strike+"), playerEnergy: 10);
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
        public static void Test_Vajra()
        {
            RunTest(name: "Strike, Strike+ works", en2: 33, cis: GetCis("Strike", "Strike+"),
                relics: GetRelics("Vajra"));
        }

        [Test]
        public static void Test_Torii()
        {
            var torii = new Torii();
            RunTest(name: "Torii works", pl2: 47,
                relics: new List<Relic>() { torii },
                amount: 5,
                count: 3);

            RunTest(name: "Torii Strong Enemy", pl2: 14,
                relics: new List<Relic>() { torii },
                enemyStatuses: GetStatuses(new Strength(), 4),
                amount: 5,
                count: 4);

            RunTest(name: "Torii Strong Enemy Weakest attakc", pl2: 46,
                relics: new List<Relic>() { torii },
                enemyStatuses: GetStatuses(new Strength(), 4),
                amount: 1,
                count: 4);
        }

        [Test]
        public void Test_PenNib()
        {
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
        public static void Test_Bash()
        {
            RunTest(name: "Enemy-attacks-bash", pl2: 20, en2: 41, cis: GetCis("Strike+"), amount: 10, count: 2,
                    playerStatuses: GetStatuses(new Vulnerable(), 3));
        }

        [Test]
        public static void Test_Disarm()
        {
            RunTest(name: "Enemy-attacks-disarmed", pl2: 43, en2: 41, cis: GetCis("Strike+", "Disarm+"), amount: 10, count: 1);
        }

        [Test]
        public static void Test_Aggressive()
        {
            var si = new List<StatusInstance>() { new StatusInstance(new Aggressive(), 4) };
            RunTest(name: "Louse-Aggressive", en2: 41, cis: GetCis("Strike+"), enemyStatuses: si, finalEnemyBlock: 4);

            var si2 = GetStatuses(new Aggressive(), 4);
            RunTest(name: "Louse-Aggressive-triggered-cleared", en2: 33, cis: GetCis("Strike+", "Inflame+", "LimitBreak", "Strike"), enemyStatuses: si2, finalEnemyBlock: 0, playerEnergy: 10);

            RunTest(name: "Enemy-attacks", pl2: 40, en2: 41, cis: GetCis("Strike+"), amount: 10, count: 1);
        }

        [Test]
        public static void Test_BurningBlood1()
        {
            var player = new Player(hpMax: 100, hp: 50, relics: GetRelics("BurningBlood"), drawAmount: 2);

            var enemy = new GenericEnemy(hpMax: 3, hp: 3);
            var initialCis = GetCis("Strike+");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();
            fight.PlayCard(initialCis[0]);
            Assert.AreEqual(fight.Status, FightStatus.Won);
            Assert.AreEqual(player.HP, 56);
        }

        [Test]
        public static void Test_BurningBlood_Overheal()
        {
            var player = new Player(hpMax: 100, hp: 99, relics: GetRelics("BurningBlood"), drawAmount: 2);

            var enemy = new GenericEnemy(hpMax: 3, hp: 3);
            var initialCis = GetCis("Strike+");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();
            fight.PlayCard(initialCis[0]);
            Assert.AreEqual(fight.Status, FightStatus.Won);
            Assert.AreEqual(player.HP, 100);
        }

        [Test]
        public static void Test_MaxEnergy_Heal()
        {
            var player = new Player(hpMax: 100, hp: 99, relics: GetRelics("BurningBlood"), maxEnergy: 1, drawAmount: 2);

            var enemy = new GenericEnemy(hpMax: 3, hp: 3);
            var initialCis = GetCis("Strike+");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();
            Assert.AreEqual(1, player.Energy);
            fight.PlayCard(initialCis[0]);
            Assert.AreEqual(0, player.Energy);
            Assert.AreEqual(player.HP, 100);
        }

        [Test]
        public static void Test_MaxDraw()
        {
            var player = new Player(relics: GetRelics("BurningBlood"), maxEnergy: 1, drawAmount: 1);

            var enemy = new GenericEnemy(hpMax: 3, hp: 3);
            var initialCis = GetCis("Strike+", "Inflame+");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();
            Assert.AreEqual(1, fight.GetHand.Count);
            fight.PlayCard(initialCis[1]);
            Assert.AreEqual(0, fight.GetHand.Count);
            Assert.AreEqual(player.HP, 100);
        }

        [Test]
        public static void Test_Sentinel()
        {
            var player = new Player(relics: GetRelics("BurningBlood"));
            var enemy = new GenericEnemy(hpMax: 3, hp: 3);
            var initialCis = GetCis("Sentinel+", "TrueGrit");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();
            fight.PlayCard(initialCis[0]);
            Assert.AreEqual(fight.Status, FightStatus.Ongoing);
            Assert.AreEqual(player.Block, 8);
            Assert.AreEqual(player.HP, 100);
        }

        [Test]
        public static void Test_WildStrike()
        {
            var player = new Player(relics: GetRelics("BurningBlood"), drawAmount: 2);
            var enemy = new GenericEnemy(hpMax: 30, hp: 30);
            var initialCis = GetCis("Sentinel+", "TrueGrit", "WildStrike");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();
            fight.PlayCard(initialCis[2]);
            Assert.AreEqual(enemy.HP, 18);
            Assert.AreEqual(2, fight.GetDrawPile().Count);
            fight.EndTurn();
            fight.StartTurn();
            Assert.AreEqual(2, fight.GetHand.Count);
            Assert.IsTrue(CompareHands(GetCis("Sentinel+", "Wound"), fight.GetHand, out var err), err);
        }

        [Test]
        public static void Test_TrueGrit()
        {
            var player = new Player(relics: GetRelics("BurningBlood"));
            var enemy = new GenericEnemy(hpMax: 3, hp: 3);
            var initialCis = GetCis("Sentinel+", "TrueGrit");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();
            fight.PlayCard(initialCis[1]);
            var hand = fight.GetHand;
            Assert.AreEqual(hand.Count, 0);
            Assert.IsTrue(CompareHands(fight.GetExhaustPile, GetCis("Sentinel+"), out string message), message);
            Assert.AreEqual(5, player.Energy);
        }

        [Test]
        public static void Test_TrueGritPlus()
        {
            var player = new Player(relics: GetRelics("BurningBlood"));
            var enemy = new GenericEnemy(hpMax: 3, hp: 3);
            var initialCis = GetCis("Sentinel+", "Inflame", "TrueGrit+");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();
            fight.PlayCard(initialCis[2], cardTargets: new List<CardInstance>() { initialCis[1] });
            var hand = fight.GetHand;
            Assert.AreEqual(hand.Count, 1);
            Assert.IsTrue(CompareHands(fight.GetHand, GetCis("Sentinel+"), out string message), message);
            Assert.IsTrue(CompareHands(fight.GetExhaustPile, GetCis("Inflame"), out string message2), message2);
            Assert.AreEqual(2, player.Energy);
        }

        [Test]
        public static void Test_FeelNoPain()
        {
            var player = new Player(relics: GetRelics("BurningBlood"));
            var enemy = new GenericEnemy(hpMax: 3, hp: 3);
            var initialCis = GetCis("FeelNoPain", "Strike", "TrueGrit+");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();
            fight.PlayCard(initialCis[0]);
            fight.PlayCard(initialCis[2]); //this should exhaust strike
            var hand = fight.GetHand;
            Assert.AreEqual(hand.Count, 0);
            Assert.IsTrue(CompareHands(fight.GetExhaustPile, GetCis("Strike"), out string message2), message2);
            Assert.AreEqual(1, player.Energy);
            Assert.IsTrue(CompareStatuses(GetStatuses(new FeelNoPainStatus(), 3), player.StatusInstances, out string se), se);
            Assert.AreEqual(13, player.Block); //TG=10 + 3 for exhaustion
        }

        [Test]
        public static void Test_Combining_FNP()
        {
            var player = new Player(relics: GetRelics("BurningBlood"), drawAmount: 6);

            var enemy = new GenericEnemy(hpMax: 3, hp: 3);
            var initialCis = GetCis("FeelNoPain", "FeelNoPain+", "Strike+", "Strike", "TrueGrit+", "TrueGrit+");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);

            fight.StartTurn();
            player.Energy = 10;
            fight.PlayCard(initialCis[0]); //FNP
            fight.PlayCard(initialCis[5], cardTargets: new List<CardInstance>() { initialCis[2] }); //exhaust the first strike. block 10+3=13
            Assert.AreEqual(13, player.Block); //TG=10 + 3 for exhaustion}
            fight.PlayCard(initialCis[1]); //FNP+
            Assert.AreEqual(13, player.Block); //TG=10 + 3 for exhaustion}
            fight.PlayCard(initialCis[4], cardTargets: new List<CardInstance>() { initialCis[3] }); // block 10+10+3+4 = 17
            var hand = fight.GetHand;
            Assert.AreEqual(0, hand.Count);
            Assert.AreEqual(2, fight.GetExhaustPile.Count);
            Assert.IsTrue(CompareStatuses(GetStatuses(new FeelNoPainStatus(), 7), player.StatusInstances, out string se), se);
            Assert.AreEqual(30, player.Block); //TG=10 + 3 for exhaustion}
        }

        [Test]
        public static void Test_BurningBlood_AtMax()
        {
            var player = new Player(hpMax: 100, hp: 100, relics: GetRelics("BurningBlood"));

            var enemy = new GenericEnemy(hpMax: 3, hp: 3);
            var initialCis = GetCis("Strike+");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();
            fight.PlayCard(initialCis[0]);
            Assert.AreEqual(fight.Status, FightStatus.Won);
            Assert.AreEqual(player.HP, 100);
        }

        [Test]
        public static void Test_Havok_Basic()
        {
            var player = new Player(drawAmount: 2);
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Strike+", "Bash+", "Havok");
            var fight = new Fight(initialCis, player: player, enemy, true);
            fight.StartTurn();

            //play havok; strike should be burned.
            fight.PlayCard(initialCis[2]);

            var dp = fight.GetDrawPile();
            Assert.AreEqual(0, dp.Count);
            Assert.AreEqual(41, enemy.HP);
            var ex = fight.GetExhaustPile;
            Assert.AreEqual(1, ex.Count);
            Assert.IsTrue(CompareHands(GetCis("Strike+"), ex, out string message), message);
        }

        /// <summary>
        /// Note - a lot of supposedly random card draws aren't actually random; the draw pile
        /// already has an order. The real randomness is previously determined state of the draw pile
        /// </summary>
        [Test]
        public static void Test_Havok_MoreCards()
        {
            var player = new Player(drawAmount: 2);
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Inflame", "Strike+", "FlameBarrier+", "Bash+", "Havok");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();

            //play havok; fb should be burned.
            fight.PlayCard(initialCis[4]);

            fight.EnemyMove(3, 3);

            var dp = fight.GetDrawPile();
            Assert.AreEqual(2, dp.Count);
            Assert.AreEqual(32, enemy.HP);
            Assert.AreEqual(100, player.HP);
            var ex = fight.GetExhaustPile;
            Assert.AreEqual(1, ex.Count);
            Assert.IsTrue(CompareHands(GetCis("FlameBarrier+"), ex, out string message), message);
            Assert.IsTrue(CompareStatuses(GetStatuses(new FlameBarrierStatus(), 6), player.StatusInstances, out string error), error);
        }

        [Test]
        public static void Test_Ectoplasm()
        {
            var player = new Player(relics: GetRelics("Ectoplasm"));
            player.GainGold(10);
            Assert.AreEqual(0, player.Gold);

            var player2 = new Player();
            player2.GainGold(10);
            Assert.AreEqual(10, player2.Gold);
        }

        [Test]
        public static void Test_EssenceOfSteel()
        {
            var potion = new EssenceOfSteel();
            var player = new Player(potions: new List<Potion>() { potion }, drawAmount: 2);
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Strike+", "Bash+", "Headbutt");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();
            fight.DrinkPotion(potion, enemy);
            Assert.IsTrue(CompareStatuses(player.StatusInstances, GetStatuses(new PlatedArmor(), 4), out string error), error);

            fight.EnemyMove(5, 2);
            //now ensure that headbutt is at the end of the draw pile.
            Assert.IsTrue(CompareStatuses(player.StatusInstances, GetStatuses(new PlatedArmor(), 2), out string error2), error2);
            Assert.AreEqual(90, player.HP, "Bad hp.");
            fight.EndTurn();
            fight.StartTurn();
            Assert.AreEqual(2, player.Block, "Should have gotten 2 essence of steel block.");
        }

        [Test]
        public static void Test_DoubleEssenceOfSteel()
        {
            var potion = new EssenceOfSteel();
            var potion2 = new EssenceOfSteel();
            var player = new Player(potions: new List<Potion>() { potion, potion2 }, drawAmount: 2);
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Strike+", "Bash+", "Headbutt");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();
            fight.DrinkPotion(potion, enemy);
            fight.DrinkPotion(potion2, enemy);

            Assert.IsTrue(CompareStatuses(player.StatusInstances, GetStatuses(new PlatedArmor(), 8), out string error), error);

            //statuses combined correctly; 8 essence - 5 attacks = 3
            fight.EnemyMove(1, 5);

            Assert.IsTrue(CompareStatuses(player.StatusInstances, GetStatuses(new PlatedArmor(), 3), out string error2), error2);
            Assert.AreEqual(95, player.HP, "Bad hp.");
            fight.EndTurn();
            fight.StartTurn();
            Assert.AreEqual(3, player.Block, "Should have gotten 2 essence of steel block.");

            fight.EndTurn();
            fight.StartTurn();
            //PA:3 HP:95

            fight.EnemyMove(1, 7);
            Assert.AreEqual(91, player.HP, "Bad hp.");
            Assert.AreEqual(0, player.Block);
            fight.EndTurn();
            Assert.AreEqual(0, player.StatusInstances.Count);
        }

        [Test]
        public static void TestHeadbutt()
        {
            var player = new Player(drawAmount: 2);
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Strike+", "Bash+", "Headbutt");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();

            var hand = fight.GetHand;

            fight.PlayCard(initialCis[1]);
            fight.PlayCard(initialCis[2], new List<CardInstance>() { initialCis[1] });
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
        }

        [Test]
        public static void TestMonkeyPaw()
        {
            var player = new Player(relics: GetRelics("MonkeyPaw"));
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Inflame+", "Bash+");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();

            //problem: when I initialize the fight I make a copy of the cards.
            fight.PlayCard(initialCis[0]);
            var hand = fight.GetHand;
            var card = hand[0];
            var ec = card.EnergyCost();
            if (ec != 0)
            {
                throw new Exception("Monkey paw didn't work");
            }
            fight.PlayCard(card);
            fight.EndTurn();
            fight.StartTurn();

            var secondHand = fight.GetHand;
            if (!CompareHands(secondHand, GetCis("Bash+"), out string message))
            {
                throw new Exception($"Invalid hand; bash should have had cost zero. {message}");
            }
        }

        [Test]
        public static void Test_Statuses()
        {
            RunTest("Slimed", cis: GetCis("Slimed", "Slimed", "Slimed"), finalEnergy: 0);
        }

        [Test]
        public static void Test_Cultist_Feather()
        {
            var player = new Player();
            var enemy = new Cultist();
            var initialCis = GetCis("Dazed", "Dazed", "Slimed");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();
            fight.EndTurn();
            fight.EnemyMove();
            //cultist should not gain strength immediately.
            Assert.IsTrue(CompareStatuses(enemy.StatusInstances, GetStatuses(new Feather(), 3), out var err), err);
        }

        [Test]
        public static void Test_Dazed()
        {
            var player = new Player();
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Dazed", "Dazed", "Slimed");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();
            fight.EndTurn();
            fight.StartTurn();
            var exhaust = fight.GetExhaustPile;
            Assert.AreEqual(exhaust.Count, 2);

            fight.PlayCard(initialCis[2]);
            fight.EndTurn();
            fight.StartTurn();
            var hand = fight.GetHand;
            Assert.AreEqual(hand.Count, 0, "Card should have exhausted");

            var exhaust2 = fight.GetExhaustPile;
            Assert.AreEqual(exhaust2.Count, 3);
        }

        [Test]
        public static void Test_Shockwave()
        {
            var player = new Player();
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Shockwave+");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();

            fight.PlayCard(initialCis[0]);
            fight.EndTurn();
            fight.StartTurn();
            fight.EndTurn();
            fight.StartTurn();
            var hand = fight.GetHand;
            Assert.AreEqual(hand.Count, 0, "Card should have exhausted");

            var exhaust = fight.GetExhaustPile;
            Assert.AreEqual(exhaust.Count, 1, "Shockwave should have exhausted failed.");
        }

        [Test]
        public static void Test_Shockwave_NotEthereal()
        {
            var player = new Player();
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Shockwave+");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();

            fight.EndTurn();
            fight.StartTurn();
            fight.EndTurn();
            fight.StartTurn();
            var hand = fight.GetHand;
            Assert.AreEqual(hand.Count, 1);

            var exhaust = fight.GetExhaustPile;
            Assert.AreEqual(exhaust.Count, 0);
        }

        [Test]
        public static void Test_Carnage_Ethereality()
        {
            var player = new Player();
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Carnage+");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();

            //problem: when I initialize the fight I make a copy of the cards.

            fight.PlayCard(initialCis[0]);
            fight.EndTurn();
            fight.StartTurn();
            fight.EndTurn();
            fight.StartTurn();
            var hand = fight.GetHand;
            Assert.AreEqual(hand.Count, 0, "Card should have exhausted due to ethereality");

            var exhaust = fight.GetExhaustPile;
            Assert.AreEqual(exhaust.Count, 1, "Carnage ethereality failed.");
        }

        [Test]
        public static void Test_Carnage_Playable_Without_Exhaustion()
        {
            var player = new Player();
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Carnage+");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();

            //problem: when I initialize the fight I make a copy of the cards.

            fight.PlayCard(initialCis[0]);
            fight.EndTurn();
            fight.StartTurn();
            fight.PlayCard(initialCis[0]);
            fight.EndTurn();
            fight.StartTurn();
            var hand = fight.GetHand;
            Assert.AreEqual(hand.Count, 1, "Card should have exhausted due to ethereality");

            var exhaust = fight.GetExhaustPile;
            Assert.AreEqual(exhaust.Count, 0, "Carnage ethereality failed.");
        }

        [Test]
        public static void TestPerfectedStrike()
        {
            var player = new Player();
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Strike+", "PerfectedStrike+", "TwinStrike");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();

            //problem: when I initialize the fight I make a copy of the cards.

            fight.PlayCard(initialCis[0]);

            //now we can play it.
            fight.PlayCard(initialCis[1]);

            var expected = 26;
            if (enemy.HP != expected)
            {
                throw new Exception($"{nameof(TestPerfectedStrike)}");
            }
        }

        [Test]
        public static void TestClash()
        {
            var player = new Player();
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Inflame+", "Clash");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();

            //problem: when I initialize the fight I make a copy of the cards.
            bool gotException;
            try
            {
                fight.PlayCard(initialCis[1]);
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

            fight.PlayCard(initialCis[0]);

            //now we can play it.
            fight.PlayCard(initialCis[1]);
        }

        [Test]
        public static void Test_PommelStrike()
        {
            var player = new Player(drawAmount: 1);
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Inflame+", "Strike+", "PommelStrike");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();

            fight.PlayCard(initialCis[2]);
            var hand = fight.GetHand;
            Assert.IsTrue(CompareHands(hand, GetCis("Strike+"), out var message), message);
        }

        [Test]
        public static void Test_PommelStrike_Override()
        {
            var player = new Player(drawAmount: 1);
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Inflame+", "Strike+", "PommelStrike");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();

            ///force us to draw the inflame
            var targets = new List<CardInstance>() { initialCis[0] };

            fight.PlayCard(initialCis[2], targets);
            var hand = fight.GetHand;
            Assert.IsTrue(CompareHands(hand, GetCis("Inflame+"), out var message), message);
        }

        [Test]
        public static void Test_PommelStrike2()
        {
            var player = new Player(drawAmount: 1);
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Inflame+", "Strike+", "PommelStrike+");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();

            fight.PlayCard(initialCis[2]);
            var hand = fight.GetHand;
            Assert.IsTrue(CompareHands(hand, GetCis("Inflame+", "Strike+"), out var message), message);
        }

        [Test]
        public static void Test_PommelStrike_Rotation()
        {
            var player = new Player(drawAmount: 2);
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Inflame+", "Strike+", "PommelStrike+");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();

            fight.PlayCard(initialCis[1]);
            fight.PlayCard(initialCis[2]);
            //hand has rotated.
            var hand = fight.GetHand;
            Assert.IsTrue(CompareHands(hand, GetCis("Inflame+", "Strike+"), out var message), message);
        }


        [Test]
        public static void Test_ShrugItOff1()
        {
            var player = new Player();
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Inflame+", "Strike+", "ShrugItOff");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();

            //problem: when I initialize the fight I make a copy of the cards.

            fight.PlayCard(initialCis[0]);
            fight.PlayCard(initialCis[1]);
            fight.PlayCard(initialCis[2]);
            var hand = fight.GetHand;
            Assert.IsTrue(CompareHands(hand, GetCis("Strike+"), out var message), message);
        }

        [Test]
        public static void Test_ShrugItOff2()
        {
            var player = new Player(drawAmount: 3);
            var enemy = new GenericEnemy();
            var initialCis = GetCis("FlameBarrier", "Inflame+", "Strike+", "ShrugItOff");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();

            fight.PlayCard(initialCis[1]);
            fight.PlayCard(initialCis[2]);
            fight.PlayCard(initialCis[3]);
            var hand = fight.GetHand;
            Assert.IsTrue(CompareHands(hand, GetCis("FlameBarrier"), out var message), message);
        }


        [Test]
        public static void Test_Sundial()
        {
            var sd = new Sundial();
            var player = new Player(relics: new List<Relic>() { sd });
            var enemy = new GenericEnemy();
            var ps = GetCi("PommelStrike");
            var s = GetCi("Strike");
            var d = GetCi("Defend");
            var fight = new Fight(new List<CardInstance>() { ps, s, d }, player: player, enemy: enemy, true);
            fight.StartTurn();
            Assert.AreEqual(0, sd.ShuffleCount);
            fight.PlayCard(s); //[] ps,s,d [] => [] ps,d s

            fight.PlayCard(ps); //=>[] d,s ps
            Assert.AreEqual(2, fight.GetHand.Count);
            Assert.AreEqual(1, fight.GetDiscardPile.Count);
            Assert.AreEqual(1, sd.ShuffleCount);
            Assert.AreEqual(1, player.Energy);

            fight.PlayCard(d); //reshuffle [] s ps,d
            Assert.AreEqual(1, fight.GetHand.Count);
            Assert.AreEqual(2, fight.GetDiscardPile.Count);
            Assert.AreEqual(1, sd.ShuffleCount);
            fight.EndTurn();
            fight.StartTurn(); //[] s,ps,d
            Assert.AreEqual(2, sd.ShuffleCount);

            fight.PlayCard(ps); //=> [] s,d ps
            Assert.AreEqual(2, fight.GetHand.Count);
            Assert.AreEqual(1, fight.GetDiscardPile.Count);
            Assert.AreEqual(3, sd.ShuffleCount);

            //bump energy
            Assert.AreEqual(4, player.Energy);
        }

        [Test]
        public static void Test_BagOfEyes()
        {
            var player = new Player();
            player.Relics.Add(Relics["BagOfEyes"]);
            var enemy = new GenericEnemy();
            var initialCis = GetCis();
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();
            Assert.True(CompareStatuses(enemy.StatusInstances, new List<StatusInstance>() { new StatusInstance(new Vulnerable(), 1) }, out string error), error);
            fight.EndTurn();
            fight.StartTurn();
            fight.EndTurn();
            fight.EnemyMove();
            Assert.True(CompareStatuses(enemy.StatusInstances, new List<StatusInstance>(), out string error2), $"Enemy status not cleared {error2}");
        }

        [Test]
        public static void Test_BagOfEyesBash()
        {
            var player = new Player();
            player.Relics.Add(Relics["BagOfEyes"]);
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Bash+");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();
            fight.PlayCard(initialCis[0]);
            Assert.True(CompareStatuses(enemy.StatusInstances, new List<StatusInstance>() { new StatusInstance(new Vulnerable(), 4) }, out string error), error);
            fight.EndTurn();
            fight.StartTurn();
            fight.EndTurn();
            fight.EnemyMove();
            Assert.True(CompareStatuses(enemy.StatusInstances, new List<StatusInstance>() { new StatusInstance(new Vulnerable(), 3) }, out string error2));
        }

        [Test]
        public static void Test_Evolve()
        {
            var player = new Player(drawAmount: 4);
            var enemy = new GenericEnemy();
            var initialCis = GetCis(
                "Strike", "Strike", "Strike", "Strike", "Wound",
                "Wound", "Strike", "Inflame", "Defend", "Strike",
                "Wound", "Evolve", "Evolve+", "PommelStrike", "PommelStrike");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn(); //E E+ PS PS
            fight.PlayCard(initialCis[12]); //evolve+
            fight.PlayCard(initialCis[13]);
            Assert.IsTrue(CompareHands(GetCis("Evolve", "PommelStrike", "Wound", "Strike", "Defend"), fight.GetHand, out var m), m);
        }

        [Test]
        public static void Test_Evolve2()
        {
            var player = new Player(drawAmount: 4);
            var enemy = new GenericEnemy();
            var initialCis = GetCis(
                "Strike", "Strike", "Strike", "Strike", "Wound",
                "Wound", "Strike", "Inflame", "Wound", "Strike",
                "Wound", "Evolve", "Evolve+", "PommelStrike", "PommelStrike");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn(); //E E+ PS PS
            fight.PlayCard(initialCis[12]); //evolve+
            fight.PlayCard(initialCis[13]);
            Assert.IsTrue(CompareHands(GetCis("Evolve", "PommelStrike", "Wound", "Strike", "Wound", "Inflame", "Strike"), fight.GetHand, out var m), m);
        }

        [Test]
        public static void Test_Evolve3()
        {
            var player = new Player(drawAmount: 4);
            var enemy = new GenericEnemy();
            var initialCis = GetCis(
                "Strike", "Strike", "Strike", "Strike", "Clash",
                "Defend", "Strike", "Inflame", "Wound", "Wound",
                "Wound", "Evolve", "Evolve+", "PommelStrike", "PommelStrike");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn(); //E E+ PS PS
            fight.PlayCard(initialCis[12]); //evolve+
            fight.PlayCard(initialCis[13]);
            Assert.IsTrue(CompareHands(GetCis("Evolve", "PommelStrike", "Wound", "Wound", "Wound", "Inflame", "Strike", "Defend", "Clash"), fight.GetHand, out var m), m);
        }

        [Test]
        public static void Test_Evolve4_Overdraw()
        {
            var player = new Player(drawAmount: 4);
            var enemy = new GenericEnemy();
            var initialCis = GetCis(
                "Strike", "Strike", "Strike", "Footwork", "Clash",
                "Defend", "Strike", "Wound", "Wound", "Wound",
                "Wound", "Evolve", "Evolve+", "PommelStrike", "PommelStrike");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn(); //E E+ PS PS
            fight.PlayCard(initialCis[12]); //evolve+
            Assert.AreEqual(0, fight.GetDiscardPile.Count);
            fight.PlayCard(initialCis[13]);
            Assert.IsTrue(CompareHands(GetCis("Evolve", "PommelStrike", "Wound", "Wound", "Wound", "Wound", "Strike", "Defend", "Clash", "Footwork"), fight.GetHand, out var m), m);

            //Strike bumped to discard
            Assert.AreEqual(2, fight.GetDiscardPile.Count);
            Assert.IsTrue(CompareHands(GetCis("PommelStrike", "Strike"), fight.GetDiscardPile, out var err), err);
        }

        [Test]
        public static void Test_Anchor()
        {
            var player = new Player();
            player.Relics.Add(Relics["Anchor"]);
            var enemy = new GenericEnemy();
            var initialCis = GetCis();
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();
            Assert.AreEqual(player.Block, 10);
            fight.EndTurn();
            fight.StartTurn();
            Assert.AreEqual(player.Block, 0);
        }

        [Test]
        public static void Test_HornCleat()
        {
            var player = new Player();
            player.Relics.Add(Relics["HornCleat"]);
            var enemy = new GenericEnemy();
            var initialCis = GetCis();
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();
            Assert.AreEqual(player.Block, 0);
            fight.EndTurn();
            fight.StartTurn();
            Assert.AreEqual(player.Block, 14);
            fight.EndTurn();
            fight.StartTurn();
            Assert.AreEqual(player.Block, 0);
        }

        [Test]
        public static void TestMonkeyPawInflames()
        {
            var player = new Player();
            player.Relics.Add(Relics["MonkeyPaw"]);
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Inflame+", "Inflame+", "Inflame+", "Inflame+");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();

            //problem: when I initialize the fight I make a copy of the cards.
            fight.PlayCard(initialCis[0]);
            var hand = fight.GetHand;

            while (hand.Count > 0)
            {
                hand = fight.GetHand;
                foreach (var card in hand)
                {
                    if (card.EnergyCost() != 0)
                    {
                        continue;
                    }
                    fight.PlayCard(card);
                    break;
                }
            }

            if (player.Energy != 2)
            {
                throw new Exception("Paw didn't work.");
            }
        }

        [Test]
        public static void Test_TheBoot()
        {
            var player = new Player();
            player.Relics.Add(Relics["TheBoot"]);
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Pummel");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();

            //problem: when I initialize the fight I make a copy of the cards.
            fight.PlayCard(initialCis[0]);
            Assert.AreEqual(30, enemy.HP);
        }

        [Test]
        public static void Test_TheBoot_FNP()
        {
            var player = new Player();
            player.Relics.Add(Relics["TheBoot"]);
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Inflame", "FeelNoPain+", "Pummel");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();

            //problem: when I initialize the fight I make a copy of the cards.
            fight.PlayCard(initialCis[0]);
            fight.PlayCard(initialCis[1]);
            fight.PlayCard(initialCis[2]);
            Assert.AreEqual(30, enemy.HP);
            Assert.AreEqual(4, player.Block);
        }

        [Test]
        public static void Test_Turnip()
        {
            var relics = GetRelics("Turnip");
            var player = new Player(relics: relics);

            var enemy = new GenericEnemy();
            var initialCis = GetCis("Inflame", "Intimidate+", "FeelNoPain+", "Pummel");
            var fight = new Fight(initialCis, player: player, enemy: enemy);
            fight.StartTurn();
            fight.EnemyMove(new EnemyAction(playerStatusAttack: GetStatuses(new Frail(), 3)));

            Assert.AreEqual(0, player.StatusInstances.Count);
        }

        [Test]
        public static void Test_Frail()
        {
            var player = new Player();
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Defend+", "Footwork+", "Footwork+");
            var fight = new Fight(initialCis, player: player, enemy: enemy);
            fight.StartTurn();
            fight.PlayCard(initialCis[0]);
            Assert.AreEqual(8, player.Block);
            fight.EndTurn();
            fight.EnemyMove(new EnemyAction(playerStatusAttack: GetStatuses(new Frail(), 3)));
            Assert.AreEqual(1, player.StatusInstances.Count);
            fight.StartTurn();
            fight.PlayCard(initialCis[0]);
            Assert.AreEqual(6, player.Block);
            fight.PlayCard(initialCis[1]);
            fight.EndTurn();
            fight.StartTurn();
            fight.PlayCard(initialCis[0]); //should be sure that the frailty is applied after the extra defense is added in.

            Assert.AreEqual(8, player.Block);
            fight.PlayCard(initialCis[2]); //should be sure that the frailty is applied after the extra defense is added in.
            fight.EndTurn();
            fight.StartTurn();
            fight.PlayCard(initialCis[0]);
            Assert.AreEqual(10, player.Block);
            Assert.AreEqual(2, player.StatusInstances.Count);
            fight.EndTurn();
            Assert.AreEqual(1, player.StatusInstances.Count);
            fight.StartTurn();
            Assert.AreEqual(1, player.StatusInstances.Count);
            fight.PlayCard(initialCis[0]);
            Assert.AreEqual(14, player.Block);
        }

        [Test]
        public static void Test_Intimidate()
        {
            var relics = GetRelics("NeowsLament", "BagOfEyes");
            var player = new Player(relics: relics);

            var enemy = new GenericEnemy();
            var initialCis = GetCis("Inflame", "Intimidate+", "FeelNoPain+", "Pummel");
            var fight = new Fight(initialCis, player: player, enemy: enemy);
            fight.StartTurn();
            fight.PlayCard(initialCis[2]);
            fight.PlayCard(initialCis[1]);
            Assert.AreEqual(4, player.Block);
            var statuses = new List<StatusInstance>();
            statuses.AddRange(GetStatuses(new Weak(), 2));
            statuses.AddRange(GetStatuses(new Vulnerable(), 1));
            Assert.IsTrue(CompareStatuses(statuses, enemy.StatusInstances, out var msg), msg);
        }

        [Test]
        public static void Test_Anger()
        {
            var player = new Player();
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Inflame", "Anger", "Pummel");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();
            fight.PlayCard(initialCis[1]);
            Assert.AreEqual(2, fight.GetDiscardPile.Count);
        }

        //TODO fix this - fix targeting.
        //[Test]
        //public static void Test_LetterOpener()
        //{
        //    var relics = GetRelics("LetterOpener");
        //    var player = new Player(relics: relics);

        //    var enemy = new GenericEnemy();
        //    var initialCis = GetCis("Pummel", "Defend", "Defend", "Defend", "Defend");
        //    var fight = new Fight(initialCis, player: player, enemy: enemy, true);
        //    fight.StartTurn();
        //    fight.PlayCard(initialCis[1]);
        //    Assert.AreEqual(50, enemy.HP);
        //    fight.PlayCard(initialCis[2]);
        //    Assert.AreEqual(50, enemy.HP);
        //    fight.PlayCard(initialCis[3]);
        //    Assert.AreEqual(45, enemy.HP);
        //    fight.PlayCard(initialCis[4]);
        //    Assert.AreEqual(45, enemy.HP);
        //    fight.PlayCard(initialCis[0]);
        //    Assert.AreEqual(37, enemy.HP);
        //}

        [Test]
        public static void Test_Neow()
        {
            var relics = GetRelics("NeowsLament");
            var player = new Player(relics: relics);

            var enemy = new GenericEnemy();
            var initialCis = GetCis("Inflame", "FeelNoPain+", "Pummel");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();
            fight.PlayCard(initialCis[0]);
            fight.PlayCard(initialCis[2]);
            Assert.AreEqual(FightStatus.Won, fight.Status);

            var enemy2 = new GenericEnemy();
            var fight2 = new Fight(initialCis, player: player, enemy: enemy2, true);
            fight2.StartTurn();
            fight2.PlayCard(initialCis[2]);
            Assert.AreEqual(FightStatus.Won, fight2.Status);

            var enemy3 = new GenericEnemy();
            var fight3 = new Fight(initialCis, player: player, enemy: enemy3, true);
            fight3.StartTurn();
            fight3.PlayCard(initialCis[2]);
            Assert.AreEqual(FightStatus.Won, fight3.Status);

            var enemy4 = new GenericEnemy();
            var fight4 = new Fight(initialCis, player: player, enemy: enemy4, true);
            fight4.StartTurn();
            fight4.PlayCard(initialCis[2]);
            Assert.AreEqual(FightStatus.Ongoing, fight4.Status);
            Assert.AreEqual(0, player.StatusInstances.Count);
        }

        [Test]
        public static void Test_Pummel()
        {
            var player = new Player();
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Pummel");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();

            //problem: when I initialize the fight I make a copy of the cards.
            fight.PlayCard(initialCis[0]);
            Assert.AreEqual(42, enemy.HP);
        }

        [Test]
        public static void Test_Enlightenment()
        {
            var player = new Player(drawAmount: 2);
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Pummel", "HeavyBlade", "Bash+", "Enlightenment", "Bash");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();

            Assert.AreEqual(2, initialCis[4].EnergyCost());
            fight.PlayCard(initialCis[3]);
            Assert.AreEqual(1, initialCis[4].EnergyCost());
            fight.EndTurn();
            fight.StartTurn();
            foreach (var ci in fight.GetDiscardPile)
            {
                Assert.IsNull(ci.PerFightOverrideEnergyCost);
                Assert.IsNull(ci.PerTurnOverrideEnergyCost);
            }
            //problem: when I initialize the fight I make a copy of the cards.
        }

        [Test]
        public static void Test_Burn()
        {
            var player = new Player(drawAmount: 2);
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Immolate", "PommelStrike+");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();
            Assert.AreEqual(0, fight.GetDiscardPile.Count);

            fight.PlayCard(initialCis[0]); // => discard = immolate + burn
            Assert.AreEqual(2, fight.GetDiscardPile.Count);

            fight.PlayCard(initialCis[1]); //=> draw immolate + burn
            Assert.AreEqual(2, fight.GetHand.Count);
            Assert.IsTrue(CompareHands(GetCis("Burn", "Immolate"), fight.GetHand, out var err), err);
            fight.EndTurn();
            Assert.AreEqual(98, player.HP);
        }

        [Test]
        public static void Test_EnlightenmentPlus()
        {
            var player = new Player(drawAmount: 2);
            var enemy = new GenericEnemy();
            var initialCis = GetCis("Pummel", "HeavyBlade", "Bash+", "Enlightenment+", "Bash");
            var fight = new Fight(initialCis, player: player, enemy: enemy, true);
            fight.StartTurn();

            Assert.AreEqual(2, initialCis[4].EnergyCost());
            fight.PlayCard(initialCis[3]);
            Assert.AreEqual(1, initialCis[4].EnergyCost());
            fight.EndTurn();
            fight.StartTurn();
            foreach (var ci in fight.GetDiscardPile)
            {
                if (ci.Card.Name == nameof(Enlightenment))
                {
                    continue;
                }
                Assert.AreEqual(1, ci.PerFightOverrideEnergyCost);
                Assert.AreEqual(1, ci.EnergyCost());
            }
            //problem: when I initialize the fight I make a copy of the cards.
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


        [Test]
        public static void FlameBarrierTests1()
        {
            RunTest(name: "FlameBarrier", pl2: 50, en2: 34, cis: GetCis("FlameBarrier"), finalPlayerBlock: 8, finalEnemyBlock: 0, amount: 1, count: 4);
        }

        [Test]
        public static void FlameBarrierTests2()
        {
            RunTest(name: "FlameBarrier-player-block1", plbl: 10, pl2: 50, en2: 36, enbl: 10, cis: GetCis("FlameBarrier+"), finalPlayerBlock: 22, finalEnemyBlock: 0, amount: 1, count: 4);
            RunTest(name: "FlameBarrier-block2", pl2: 50, finalPlayerBlock: 12, en2: 36, enbl: 10, cis: GetCis("FlameBarrier+"), finalEnemyBlock: 0, amount: 1, count: 4);
            RunTest(name: "FlameBarrier-block3", pl2: 50, enbl: 41, finalPlayerBlock: 24, finalEnemyBlock: 1, cis: GetCis("Inflame+", "Inflame+", "FlameBarrier+", "FlameBarrier"), amount: 1, count: 4, playerEnergy: 10);
            RunTest(name: "FlameBarrier-block4", pl2: 50, enbl: 39, en2: 49, finalPlayerBlock: 24, finalEnemyBlock: 0, cis: GetCis("Inflame+", "Inflame+", "FlameBarrier+", "FlameBarrier"), amount: 1, count: 4, playerEnergy: 10);
        }

        [Test]
        public static void DamageBlockTests()
        {
            //TODO this needs fixing.  When the player is weak and target is vuln, do we math.floor both times? or just once at the end.
            RunTest(name: "Weak-Inflame-BodySlam-Vulned", en2: 30, finalPlayerBlock: 10, cis: GetCis("Footwork", "Defend+", "Bash", "Inflame+", "BodySlam+"), playerStatuses: GetStatuses(new Weak(), 2), playerEnergy: 10);

            RunTest(name: "BodySlam", en2: 40, finalPlayerBlock: 10, cis: GetCis("Footwork", "Defend+", "BodySlam+"));
            RunTest(name: "BodySlam-Vulned", en2: 27, finalPlayerBlock: 10, cis: GetCis("Footwork", "Defend+", "Bash", "BodySlam+"), playerEnergy: 10);
            RunTest(name: "Inflame-BodySlam-Vulned", en2: 23, finalPlayerBlock: 10, cis: GetCis("Footwork", "Defend+", "Bash", "Inflame+", "BodySlam+"), playerEnergy: 10);



            RunTest(name: "ClearingEnemyBlock", en2: 34, enbl: 10, cis: GetCis("Strike+", "Bash", "Strike"), finalEnemyBlock: 0, playerEnergy: 10);
            RunTest(name: "FullBlocked", en2: 50, enbl: 26, cis: GetCis("Strike+", "Bash", "Strike"), finalEnemyBlock: 0, playerEnergy: 10);
            RunTest(name: "VulnBreakThrough", en2: 49, enbl: 16, cis: GetCis("Bash", "Strike"), finalEnemyBlock: 0);
            RunTest(name: "HeavyBlade", en2: 27, enbl: 20, cis: GetCis("HeavyBlade", "Inflame+", "HeavyBlade+"), finalEnemyBlock: 0, playerEnergy: 10);

            RunTest(name: "double-FlameBarrier-block", pl2: 50, finalPlayerBlock: 24, enbl: 0, en2: 10,
                cis: GetCis("Inflame+", "Inflame+", "FlameBarrier+", "FlameBarrier"), amount: 1, count: 4, playerEnergy: 10);
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
            var fight = new Fight(cis, pl, en);
            fight.StartTurn();

            while (true)
            {

                while (true)
                {
                    var hand = fight.GetHand;
                    if (hand.Count == 0)
                    {
                        break;
                    }
                    fight.PlayCard(hand[0]);
                    if (fight._Enemies[0].Dead)
                    {
                        break;
                    }
                }

                if (fight.Status != FightStatus.Ongoing)
                {
                    break;
                }
                fight.EnemyMove();
                if (fight.Status != FightStatus.Ongoing)
                {
                    break;
                }


                fight.EndTurn();
                fight.StartTurn();
            }
        }

        public static void TestDrawOnly(string testName, List<CardInstance> initialCis, List<CardInstance> expectedCis,
            int drawCount = 5, int extraTurns = 0, int? energyAfter = null, CardDomain? characterType = CardDomain.IronClad)
        {
            var player = new Player(characterType.Value, drawAmount: drawCount);
            var enemy = new GenericEnemy();
            var fight = new Fight(initialCis, player, enemy: enemy, true);

            //initial card draw.
            fight.StartTurn();

            while (extraTurns > 0)
            {
                fight.EndTurn();
                fight.StartTurn();
                extraTurns--;
            }

            var hand = fight.GetHand;
            Assert.IsTrue(CompareHands(hand, expectedCis, out string message), message);

            if (energyAfter.HasValue)
            {
                Assert.AreEqual(player.Energy, energyAfter, $"Expected energy={energyAfter.Value} actual={player.Energy}");
            }
        }


        public static EnemyAction Attack(int amount, int count)
        {
            return new EnemyAction(null, new EnemyAttack(amount, count), null);
        }

        public static EnemyAction GetEnemyAction(EnemyAttack card)
        {
            return new EnemyAction(null, card, null);
        }

        public static List<IEnemy> SetupEnemies(string en, int ehp, int enbl, List<StatusInstance> enemyStatuses)
        {
            var enemy = new GenericEnemy(ehp, ehp);
            enemy.Block = enbl;

            enemy.StatusInstances = enemyStatuses ?? new List<StatusInstance>();

            var enemies = new List<IEnemy>() { enemy };
            return enemies;
        }

    }
}