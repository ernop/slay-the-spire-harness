using System;
using System.Collections.Generic;

using static StS.AllCards;
using static StS.AllRelics;
using static StS.Helpers;

namespace StS
{
    public static class Tests
    {
        public static Dictionary<string, Card> Cards = GetAllCards();
        public static Dictionary<string, Relic> Relics = GetAllRelics();

        public static void DoTest(string name, int pl = 50, int pl2 = 50, int en = 50, int en2 = 50,
           int plbl = 0, int enbl = 0, int finalPlayerBlock = 0, int finalEnemyBlock = 0,
           List<Relic> relics = null, List<CardInstance> cis = null, List<CardInstance> enemyCards = null,
           List<StatusInstance> playerStatuses = null, List<StatusInstance> enemyStatuses = null,
           List<StatusInstance> playerFinalStatuses = null,
           List<StatusInstance> enemyFinalStatuses = null,
           int? playerEnergy = null,
           int? finalEnergy = null)
        {
            var tc = new FightTestCase()
            {
                EnemyHp = en,
                PlayerBlock = plbl,
                EnemyBlock = enbl,
                FinalEnemyHp = en2,
                PlayerHp = pl,
                PlayerEnergy = playerEnergy,
                FinalPlayerHp = pl2,
                TestName = name,
                CardsToPlay = cis ?? new List<CardInstance>(),
                FinalPlayerBlock = finalPlayerBlock,
                FinalEnemyBlock = finalEnemyBlock,
                Relics = relics ?? new List<Relic>(),
                PlayerStatuses = playerStatuses ?? new List<StatusInstance>(),
                EnemyStatuses = enemyStatuses ?? new List<StatusInstance>(),
                EnemyCards = enemyCards ?? new List<CardInstance>(),
                PlayerFinalStatuses = playerFinalStatuses ?? new List<StatusInstance>(),
                EnemyFinalStatuses = enemyFinalStatuses ?? new List<StatusInstance>(),
                FinalEnergy = finalEnergy
            };
            tc.Run();
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

        public static void BasicTests()
        {
            var statuses = new List<StatusInstance>() { new StatusInstance(new Vulnerable(), 8), new StatusInstance(new Weak(), 8) };
            DoTest("Shockwave+VulnWeakAttack", en2: 37, pl2: 26, cis: GetCis("Shockwave", "Shockwave+", "Strike+"), enemyFinalStatuses: statuses, enemyCards: Attack(8, 4), playerEnergy: 10);

            DoTest("BashIronwave", en2: 32, cis: GetCis("Bash", "IronWave+"), finalPlayerBlock: 7);
            DoTest("BashInflameIronwave", en2: 27, cis: GetCis("Bash", "Inflame+", "IronWave+"), finalPlayerBlock: 7, playerEnergy: 10);

            DoTest("DefendNeg3", cis: GetCis("Footwork", "Defend"), finalPlayerBlock: 2, playerStatuses: new List<StatusInstance>() { new StatusInstance(new Dexterity(), -5) });
            DoTest("DefendNeg5", cis: GetCis("Footwork", "Defend"), finalPlayerBlock: 0, playerStatuses: new List<StatusInstance>() { new StatusInstance(new Dexterity(), -7) });
            DoTest("DefendNeg2", cis: GetCis("Footwork", "Defend"), finalPlayerBlock: 3, playerStatuses: new List<StatusInstance>() { new StatusInstance(new Dexterity(), -4) });
            DoTest("Defend", cis: GetCis("Defend"), finalPlayerBlock: 5);
            DoTest("FootDefend", cis: GetCis("Footwork", "Defend"), finalPlayerBlock: 7);
            DoTest("Foot+Defend", cis: GetCis("Footwork+", "Defend"), finalPlayerBlock: 8);
            DoTest("FootFoot+Defend", cis: GetCis("Footwork", "Footwork+", "Defend"), finalPlayerBlock: 10);
            DoTest("Strike works", en2: 44, cis: GetCis("Strike"));
            DoTest("Strike, Strike+ works", en2: 35, cis: GetCis("Strike", "Strike+"));


            DoTest("Bashing", en2: 19, cis: GetCis("Strike", "Bash+", "Strike+"), playerEnergy: 10);
            DoTest("SwordBoomerang", en2: 41, cis: GetCis("SwordBoomerang"));
            DoTest("SwordBoomerang+", en2: 38, cis: GetCis("SwordBoomerang+"));
            DoTest("SwordBoomerang+ vs block1", en2: 39, cis: GetCis("SwordBoomerang+"), enbl: 1, finalEnemyBlock: 0);
            DoTest("SwordBoomerang+ vs block2", en2: 40, cis: GetCis("SwordBoomerang+"), enbl: 2, finalEnemyBlock: 0);
            DoTest("SwordBoomerang+ vs block3", en2: 41, cis: GetCis("SwordBoomerang+"), enbl: 3, finalEnemyBlock: 0);
            DoTest("SwordBoomerang+ vs block4", en2: 42, cis: GetCis("SwordBoomerang+"), enbl: 4, finalEnemyBlock: 0);
            DoTest("SwordBoomerang+ vs block13", cis: GetCis("SwordBoomerang+"), enbl: 13, finalEnemyBlock: 1);
            DoTest("Inflamed SwordBoomerang+", en2: 30, cis: GetCis("Inflame", "SwordBoomerang+"));

            DoTest("Inflame", en2: 35, cis: GetCis("Strike", "Inflame+", "Strike"));
            DoTest("Inflame+Bash", en2: 39, cis: GetCis("Inflame+", "Bash"));
            DoTest("Inflame+Bash+strike", en2: 26, cis: GetCis("Inflame+", "Bash", "Strike"), playerEnergy: 10);
            DoTest("Inflame+LimitBreak", en2: 32, cis: GetCis("Strike", "Inflame+", "LimitBreak", "Strike"), playerEnergy: 10);
            DoTest("Inflame+LimitBreak+bash", en2: 8, cis: GetCis("Strike", "Inflame+", "LimitBreak", "Bash", "Strike+"), playerEnergy: 10);

            DoTest("WeakPlayer", cis: GetCis("Strike"), en2: 46, playerStatuses: new List<StatusInstance>() { new StatusInstance(new Weak(), 3) });
            DoTest("WeakenEnemy", cis: GetCis("Uppercut+"), en2: 37,
                enemyFinalStatuses: new List<StatusInstance>() { new StatusInstance(new Vulnerable(), 2), new StatusInstance(new Weak(), 2) });

            DoTest("Entrench", cis: GetCis("Footwork+", "Defend+", "Entrench", "Defend"), finalPlayerBlock: 30, playerEnergy: 10);
        }

        public static void RelicTests()
        {
            var vajraOne = new Vajra { Intensity = 1 };
            DoTest("Strike, Strike+ works", en2: 33, cis: GetCis("Strike", "Strike+"),
                relics: new List<Relic>() { vajraOne });

            var torii = new Torii();
            DoTest("Torii works", pl2: 47,
                relics: new List<Relic>() { torii },
                enemyCards: new List<CardInstance>() {
                new CardInstance(new EnemyAttack(5,3), 0)});

            DoTest("Torii Strong Enemy", pl2: 10,
                relics: new List<Relic>() { torii },
                enemyStatuses: new List<StatusInstance>() { new StatusInstance(new Strength(), 4) },
                enemyCards: new List<CardInstance>() {
                new CardInstance(new EnemyAttack(6,4),0)});

            DoTest("Torii Strong Enemy Weakest attakc", pl2: 46,
                relics: new List<Relic>() { torii },
                enemyStatuses: new List<StatusInstance>() { new StatusInstance(new Strength(), 4) },
                enemyCards: new List<CardInstance>() {
                new CardInstance(new EnemyAttack(1,4),0)});

            TestMonkeyPawInflames();
            TestMonkeyPaw();

            var penNib = new PenNib();
            penNib.AttackCount = 8;
            DoTest("PenNib toggles-untoggles", en2: 20, cis: GetCis("Strike", "Strike+", "Strike"), relics: new List<Relic>() { penNib });

            var penNib2 = new PenNib();
            penNib2.AttackCount = 2;
            DoTest("PenNib-Nonfunctional", en2: 35, cis: GetCis("Strike", "Strike+"), relics: new List<Relic>() { penNib2 });

            var penNib3 = new PenNib();
            penNib3.AttackCount = 8;
            DoTest("PenNib-Inflame", en2: 20, cis: GetCis("Strike", "Inflame+", "Strike+"), relics: new List<Relic>() { penNib3 });

            var penNib4 = new PenNib();
            penNib4.AttackCount = 8;
            DoTest("Inflamed Nibbed SwordBoomerang+", en2: 4, cis: GetCis("Strike", "Inflame", "SwordBoomerang+"), relics: new List<Relic>() { penNib4 });
        }

        public static void EnemyBehaviorTests()
        {
            DoTest("Enemy-attacks-disarmed", pl2: 43, en2: 41, cis: GetCis("Strike+", "Disarm+"), enemyCards: new List<CardInstance>() { new CardInstance(new EnemyAttack(10, 1), 0) });

            DoTest("Enemy-attacks-bash", pl2: 20, en2: 41, cis: GetCis("Strike+"), enemyCards: new List<CardInstance>() {
                new CardInstance(new EnemyAttack(10, 2), 0) },
                playerStatuses: new List<StatusInstance>() { new StatusInstance(new Vulnerable(), 3) });

            var si = new List<StatusInstance>() { new StatusInstance(new Aggressive(), 4) };
            DoTest("Louse-Aggressive", en2: 41, cis: GetCis("Strike+"), enemyStatuses: si, finalEnemyBlock: 4);

            var si2 = new List<StatusInstance>() { new StatusInstance(new Aggressive(), 4) };
            DoTest("Louse-Aggressive-triggered-cleared", en2: 33, cis: GetCis("Strike+", "Inflame+", "LimitBreak", "Strike"), enemyStatuses: si2, finalEnemyBlock: 0, playerEnergy: 10);

            DoTest("Enemy-attacks", pl2: 40, en2: 41, cis: GetCis("Strike+"), enemyCards: Attack(10, 1));


        }

        public static void TestHeadbutt()
        {
            Console.WriteLine($"Starting{nameof(TestHeadbutt)}");
            var player = new Player();
            var gc = new GameContext();
            var enemy = new Enemy();
            var initialCis = GetCis("Strike+", "Bash+", "Headbutt");
            var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
            fight.NextTurn(2);

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

        public static void TestMonkeyPaw()
        {
            Console.WriteLine($"Starting{nameof(TestMonkeyPaw)}");
            var player = new Player();
            player.Relics.Add(Relics["MonkeyPaw"]);
            var gc = new GameContext();
            var enemy = new Enemy();
            var initialCis = GetCis("Inflame+", "Bash+");
            var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
            fight.NextTurn(5);

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

        public static void TestPerfectedStrike()
        {
            Console.WriteLine($"Starting{nameof(TestPerfectedStrike)}");
            var player = new Player();
            var gc = new GameContext();
            var enemy = new Enemy();
            var initialCis = GetCis("Strike+", "PerfectedStrike+", "TwinStrike");
            var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
            fight.NextTurn(5);

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

        public static void TestClash()
        {
            var player = new Player();
            var gc = new GameContext();
            var enemy = new Enemy();
            var initialCis = GetCis("Inflame+", "Clash");
            var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
            fight.NextTurn(5);

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

        public static void TestShrugItOff()
        {
            {
                var player = new Player();
                var gc = new GameContext();
                var enemy = new Enemy();
                var initialCis = GetCis("Inflame+", "Strike+", "ShrugItOff");
                var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
                fight.NextTurn(3);

                //problem: when I initialize the fight I make a copy of the cards.

                fight.PlayCard(initialCis[0], player, enemy);
                fight.PlayCard(initialCis[1], player, enemy);
                fight.PlayCard(initialCis[2], player, enemy);
                var hand = fight.GetHand();
                if (!CompareHands(hand, GetCis("Strike+"), out var message))
                {
                    throw new Exception($"{nameof(TestShrugItOff)}1 failed to draw. {message}");
                }
            }
            {
                var player = new Player();
                var gc = new GameContext();
                var enemy = new Enemy();
                var initialCis = GetCis("FlameBarrier", "Inflame+", "Strike+", "ShrugItOff");
                var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
                fight.NextTurn(3);

                fight.PlayCard(initialCis[1], player, enemy);
                fight.PlayCard(initialCis[2], player, enemy);
                fight.PlayCard(initialCis[3], player, enemy);
                var hand = fight.GetHand();
                if (!CompareHands(hand, GetCis("FlameBarrier"), out var message))
                {
                    throw new Exception($"{nameof(TestShrugItOff)}2 failed to draw. {message}");
                }
            }
            //test 2 - pulling from one.
        }

        public static void TestMonkeyPawInflames()
        {
            var player = new Player();
            player.Relics.Add(Relics["MonkeyPaw"]);
            var gc = new GameContext();
            var enemy = new Enemy();
            var initialCis = GetCis("Inflame+", "Inflame+", "Inflame+", "Inflame+");
            var fight = new Fight(initialCis, gameContext: gc, player: player, enemies: new List<Enemy>() { enemy }, true);
            fight.NextTurn(5);

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
            fight.NextTurn(drawCount);

            while (extraTurns > 0)
            {
                fight.NextTurn(drawCount);
                extraTurns--;
            }

            var hand = fight.GetHand();
            if (!CompareHands(hand, expectedCis, out string message))
            {
                throw new Exception($"{testName} failed: {message}");
            }

            if (energyAfter.HasValue)
            {
                if (player.Energy != energyAfter)
                {
                    throw new Exception($"Expected energy={energyAfter.Value} actual={player.Energy}");
                }
            }

            Console.WriteLine($"Test: {testName} passed.");
        }


        public static void DamageBlockTests()
        {

            //TODO this needs fixing.  When the player is weak and target is vuln, do we math.floor both times? or just once at the end.
            DoTest("Weak-Inflame-BodySlam-Vulned", en2: 30, finalPlayerBlock: 10, cis: GetCis("Footwork", "Defend+", "Bash", "Inflame+", "BodySlam+"), playerStatuses: GetStatuses(new Weak(), 2), playerEnergy: 10);

            DoTest("BodySlam", en2: 40, finalPlayerBlock: 10, cis: GetCis("Footwork", "Defend+", "BodySlam+"));
            DoTest("BodySlam-Vulned", en2: 27, finalPlayerBlock: 10, cis: GetCis("Footwork", "Defend+", "Bash", "BodySlam+"), playerEnergy: 10);
            DoTest("Inflame-BodySlam-Vulned", en2: 23, finalPlayerBlock: 10, cis: GetCis("Footwork", "Defend+", "Bash", "Inflame+", "BodySlam+"), playerEnergy: 10);


            DoTest("FlameBarrier", pl2: 50, en2: 34, cis: GetCis("FlameBarrier"), finalPlayerBlock: 8, finalEnemyBlock: 0, enemyCards: Attack(1, 4));
            DoTest("FlameBarrier-player-block1", plbl: 10, pl2: 50, en2: 36, enbl: 10, cis: GetCis("FlameBarrier+"), finalPlayerBlock: 22, finalEnemyBlock: 0, enemyCards: Attack(1, 4));
            DoTest("FlameBarrier-block2", pl2: 50, finalPlayerBlock: 12, en2: 36, enbl: 10, cis: GetCis("FlameBarrier+"), finalEnemyBlock: 0, enemyCards: Attack(1, 4));
            DoTest("FlameBarrier-block3", pl2: 50, enbl: 41, finalPlayerBlock: 24, finalEnemyBlock: 1, cis: GetCis("Inflame+", "Inflame+", "FlameBarrier+", "FlameBarrier"), enemyCards: Attack(1, 4), playerEnergy: 10);
            DoTest("FlameBarrier-block4", pl2: 50, enbl: 39, en2: 49, finalPlayerBlock: 24, finalEnemyBlock: 0, cis: GetCis("Inflame+", "Inflame+", "FlameBarrier+", "FlameBarrier"), enemyCards: Attack(1, 4), playerEnergy: 10);

            DoTest("ClearingEnemyBlock", en2: 34, enbl: 10, cis: GetCis("Strike+", "Bash", "Strike"), finalEnemyBlock: 0, playerEnergy: 10);
            DoTest("FullBlocked", en2: 50, enbl: 26, cis: GetCis("Strike+", "Bash", "Strike"), finalEnemyBlock: 0, playerEnergy: 10);
            DoTest("VulnBreakThrough", en2: 49, enbl: 16, cis: GetCis("Bash", "Strike"), finalEnemyBlock: 0);
            DoTest("HeavyBlade", en2: 27, enbl: 20, cis: GetCis("HeavyBlade", "Inflame+", "HeavyBlade+"), finalEnemyBlock: 0, playerEnergy: 10);

            DoTest("double-FlameBarrier-block", pl2: 50, finalPlayerBlock: 24, enbl: 0, en2: 10,
                cis: GetCis("Inflame+", "Inflame+", "FlameBarrier+", "FlameBarrier"), enemyCards: Attack(1, 4), playerEnergy: 10);
            DoTest("Combining statuses works", finalPlayerBlock: 28, cis: GetCis("Inflame+", "Inflame", "FlameBarrier+", "FlameBarrier"),
                playerFinalStatuses: new List<StatusInstance>() {
                    new StatusInstance(new Strength(), 5),
                    new StatusInstance(new FlameBarrierStatus(), 10)},
                playerEnergy: 10);

            DoTest("BustedCrown-Gives", cis: GetCis(),
                finalEnergy: 4,
                relics: new List<Relic>() { Relics["BustedCrown"] });

            DoTest("Start-3energy", cis: GetCis(),
                finalEnergy: 3,
                relics: new List<Relic>() { Relics["MonkeyPaw"] });

            DoTest("Start-3-paw", cis: GetCis("Inflame", "Strike"),
                finalEnergy: 2,
                relics: new List<Relic>() { Relics["MonkeyPaw"] }, en2: 42);

            DoTest("Start-4-paw", cis: GetCis("Inflame", "Strike"),
                finalEnergy: 3,
                relics: new List<Relic>() { Relics["BustedCrown"], Relics["MonkeyPaw"] }, en2: 42);

            DoTest("Start-4-paw-zero-out-nothing", cis: GetCis("Strike", "Inflame"),
                finalEnergy: 2,
                relics: new List<Relic>() { Relics["BustedCrown"], Relics["MonkeyPaw"] }, en2: 44);

            DoTest("Energy-relics-combine", cis: GetCis("Strike", "Inflame"),
                finalEnergy: 3,
                relics: new List<Relic>() { Relics["FusionHammer"], Relics["BustedCrown"] }, en2: 44);

            DoTest("Energy-relics-combine2", cis: GetCis("Strike", "Inflame"),
                finalEnergy: 3,
                relics: new List<Relic>() { Relics["BustedCrown"], Relics["FusionHammer"] }, en2: 44);

        }

        public static List<StatusInstance> GetStatuses(Status status, int num)
        {
            return new List<StatusInstance>() { new StatusInstance(status, num) };
        }

        public static List<CardInstance> Attack(int amount, int count)
        {
            return new List<CardInstance>() { new CardInstance(new EnemyAttack(amount, count), 0) };
        }

        public static void CardTests()
        {
            TestClash();
            TestHeadbutt();
            TestPerfectedStrike();
            TestShrugItOff();
        }

        public static void RunTests(bool print)
        {
            PrintDetails = print;

            CardTests();

            DrawTests();

            if (true)
            {
                DamageBlockTests();
                BasicTests();
                RelicTests();

                EnemyBehaviorTests();
            }

            Console.ReadLine();
        }
    }
}