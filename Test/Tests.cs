using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using static StS.AllCards;


namespace StS
{
    public static class Tests
    {
        public static Dictionary<string, Card> CardList = GetAllCards();

        /// <summary>
        /// Test exhausts, draws, etc.
        /// </summary>
        public class CardTestCase
        {
            public Deck BeforeDeck { get; set; }
            public Deck AfterDeck { get; set; }
        }

        /// <summary>
        /// Are the decks identical? for use in testing
        /// </summary>
        public static bool CompareDecks(Deck a, Deck b)
        {
            return true;
        }

        public static void DoTest(string name, int pl = 50, int pl2 = 50, int en = 50, int en2 = 50,
            int plbl = 0, int enbl = 0, int finalPlayerBlock = 0, int finalEnemyBlock = 0,
            List<Relic> relics = null, List<CardInstance> cis = null, List<CardInstance> enemyCards = null,
            List<StatusInstance> playerStatuses = null, List<StatusInstance> enemyStatuses = null,
            List<StatusInstance> playerFinalStatuses = null,
            List<StatusInstance> enemyFinalStatuses = null)
        {
            var tc = new FightTestCase()
            {
                EnemyHp = en,
                PlayerBlock = plbl,
                EnemyBlock = enbl,
                FinalEnemyHp = en2,
                PlayerHp = pl,
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
                EnemyFinalStatuses = enemyFinalStatuses ?? new List<StatusInstance>()
            };
            tc.Run();
        }

        public static List<CardInstance> GetCi(params string[] names)
        {
            var cis = new List<CardInstance>();
            foreach (var name in names)
            {
                var x = Helpers.SplitCardName(name);
                var card = CardList[x.Item1];
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
            DoTest("Shockwave+VulnWeakAttack", en2: 37, pl2: 26, cis: GetCi("Shockwave", "Shockwave+", "Strike+"), enemyFinalStatuses: statuses, enemyCards: Attack(8, 4));

            DoTest("BashIronwave", en2: 32, cis: GetCi("Bash", "IronWave+"), finalPlayerBlock: 7);
            DoTest("BashInflameIronwave", en2: 27, cis: GetCi("Bash", "Inflame+", "IronWave+"), finalPlayerBlock: 7);

            DoTest("DefendNeg3", cis: GetCi("Footwork", "Defend"), finalPlayerBlock: 2, playerStatuses: new List<StatusInstance>() { new StatusInstance(new Dexterity(), -5) });
            DoTest("DefendNeg5", cis: GetCi("Footwork", "Defend"), finalPlayerBlock: 0, playerStatuses: new List<StatusInstance>() { new StatusInstance(new Dexterity(), -7) });
            DoTest("DefendNeg2", cis: GetCi("Footwork", "Defend"), finalPlayerBlock: 3, playerStatuses: new List<StatusInstance>() { new StatusInstance(new Dexterity(), -4) });
            DoTest("Defend", cis: GetCi("Defend"), finalPlayerBlock: 5);
            DoTest("FootDefend", cis: GetCi("Footwork", "Defend"), finalPlayerBlock: 7);
            DoTest("Foot+Defend", cis: GetCi("Footwork+", "Defend"), finalPlayerBlock: 8);
            DoTest("FootFoot+Defend", cis: GetCi("Footwork", "Footwork+", "Defend"), finalPlayerBlock: 10);
            DoTest("Strike works", en2: 44, cis: GetCi("Strike"));
            DoTest("Strike, Strike+ works", en2: 35, cis: GetCi("Strike", "Strike+"));


            DoTest("Bashing", en2: 19, cis: GetCi("Strike", "Bash+", "Strike+"));
            DoTest("SwordBoomerang", en2: 41, cis: GetCi("SwordBoomerang"));
            DoTest("SwordBoomerang+", en2: 38, cis: GetCi("SwordBoomerang+"));
            DoTest("SwordBoomerang+ vs block1", en2: 39, cis: GetCi("SwordBoomerang+"), enbl: 1, finalEnemyBlock: 0);
            DoTest("SwordBoomerang+ vs block2", en2: 40, cis: GetCi("SwordBoomerang+"), enbl: 2, finalEnemyBlock: 0);
            DoTest("SwordBoomerang+ vs block3", en2: 41, cis: GetCi("SwordBoomerang+"), enbl: 3, finalEnemyBlock: 0);
            DoTest("SwordBoomerang+ vs block4", en2: 42, cis: GetCi("SwordBoomerang+"), enbl: 4, finalEnemyBlock: 0);
            DoTest("SwordBoomerang+ vs block13", cis: GetCi("SwordBoomerang+"), enbl: 13, finalEnemyBlock: 1);
            DoTest("Inflamed SwordBoomerang+", en2: 30, cis: GetCi("Inflame", "SwordBoomerang+"));

            DoTest("Inflame", en2: 35, cis: GetCi("Strike", "Inflame+", "Strike"));
            DoTest("Inflame+Bash", en2: 39, cis: GetCi("Inflame+", "Bash"));
            DoTest("Inflame+Bash+strike", en2: 26, cis: GetCi("Inflame+", "Bash", "Strike"));
            DoTest("Inflame+LimitBreak", en2: 32, cis: GetCi("Strike", "Inflame+", "LimitBreak", "Strike"));
            DoTest("Inflame+LimitBreak+bash", en2: 8, cis: GetCi("Strike", "Inflame+", "LimitBreak", "Bash", "Strike+"));

            DoTest("WeakPlayer", cis: GetCi("Strike"), en2: 46, playerStatuses: new List<StatusInstance>() { new StatusInstance(new Weak(), 3) });
            DoTest("WeakenEnemy", cis: GetCi("Uppercut+"), en2: 37,
                enemyFinalStatuses: new List<StatusInstance>() { new StatusInstance(new Vulnerable(), 2), new StatusInstance(new Weak(), 2) });

            DoTest("Entrench", cis: GetCi("Footwork+", "Defend+", "Entrench", "Defend"), finalPlayerBlock: 30);
        }

        public static void RelicTests()
        {
            var vajraOne = new Vajra { Intensity = 1 };
            DoTest("Strike, Strike+ works", en2: 33, cis: GetCi("Strike", "Strike+"),
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
        }

        public static void PenNibTests()
        {
            var penNib = new PenNib();
            penNib.AttackCount = 8;

            DoTest("PenNib toggles-untoggles", en2: 20, cis: GetCi("Strike", "Strike+", "Strike"), relics: new List<Relic>() { penNib });

            var penNib2 = new PenNib();
            penNib2.AttackCount = 2;

            DoTest("PenNib-Nonfunctional", en2: 35, cis: GetCi("Strike", "Strike+"), relics: new List<Relic>() { penNib2 });


            var penNib3 = new PenNib();
            penNib3.AttackCount = 8;

            DoTest("PenNib-Inflame", en2: 20, cis: GetCi("Strike", "Inflame+", "Strike+"), relics: new List<Relic>() { penNib3 });

            var penNib4 = new PenNib();
            penNib4.AttackCount = 8;
            DoTest("Inflamed Nibbed SwordBoomerang+", en2: 4, cis: GetCi("Strike", "Inflame", "SwordBoomerang+"), relics: new List<Relic>() { penNib4 });
        }

        public static void EnemyBehaviorTests()
        {
            DoTest("Enemy-attacks-disarmed", pl2: 43, en2: 41, cis: GetCi("Strike+", "Disarm+"), enemyCards: new List<CardInstance>() { new CardInstance(new EnemyAttack(10, 1), 0) });

            DoTest("Enemy-attacks-bash", pl2: 20, en2: 41, cis: GetCi("Strike+"), enemyCards: new List<CardInstance>() {
                new CardInstance(new EnemyAttack(10, 2), 0) },
                playerStatuses: new List<StatusInstance>() { new StatusInstance(new Vulnerable(), 3) });

            var si = new List<StatusInstance>() { new StatusInstance(new Aggressive(), 4) };
            DoTest("Louse-Aggressive", en2: 41, cis: GetCi("Strike+"), enemyStatuses: si, finalEnemyBlock: 4);

            var si2 = new List<StatusInstance>() { new StatusInstance(new Aggressive(), 4) };
            DoTest("Louse-Aggressive-triggered-cleared", en2: 33, cis: GetCi("Strike+", "Inflame+", "LimitBreak", "Strike"), enemyStatuses: si2, finalEnemyBlock: 0);

            DoTest("Enemy-attacks", pl2: 40, en2: 41, cis: GetCi("Strike+"), enemyCards: Attack(10, 1));


        }

        public static void DamageBlockTests()
        {

            //TODO this needs fixing.  When the player is weak and target is vuln, do we math.floor both times? or just once at the end.
            DoTest("Weak-Inflame-BodySlam-Vulned", en2: 30, finalPlayerBlock: 10, cis: GetCi("Footwork", "Defend+", "Bash", "Inflame+", "BodySlam+"), playerStatuses: GetStatuses(new Weak(), 2));

            DoTest("BodySlam", en2: 40, finalPlayerBlock: 10, cis: GetCi("Footwork", "Defend+", "BodySlam+"));
            DoTest("BodySlam-Vulned", en2: 27, finalPlayerBlock: 10, cis: GetCi("Footwork", "Defend+", "Bash", "BodySlam+"));
            DoTest("Inflame-BodySlam-Vulned", en2: 23, finalPlayerBlock: 10, cis: GetCi("Footwork", "Defend+", "Bash", "Inflame+", "BodySlam+"));


            DoTest("FlameBarrier", pl2: 50, en2: 34, cis: GetCi("FlameBarrier"), finalPlayerBlock: 8, finalEnemyBlock: 0, enemyCards: Attack(1, 4));
            DoTest("FlameBarrier-player-block1", plbl: 10, pl2: 50, en2: 36, enbl: 10, cis: GetCi("FlameBarrier+"), finalPlayerBlock: 22, finalEnemyBlock: 0, enemyCards: Attack(1, 4));
            DoTest("FlameBarrier-block2", pl2: 50, finalPlayerBlock: 12, en2: 36, enbl: 10, cis: GetCi("FlameBarrier+"), finalEnemyBlock: 0, enemyCards: Attack(1, 4));
            DoTest("FlameBarrier-block3", pl2: 50, enbl: 41, finalPlayerBlock: 24, finalEnemyBlock: 1, cis: GetCi("Inflame+", "Inflame+", "FlameBarrier+", "FlameBarrier"), enemyCards: Attack(1, 4));
            DoTest("FlameBarrier-block4", pl2: 50, enbl: 39, en2: 49, finalPlayerBlock: 24, finalEnemyBlock: 0, cis: GetCi("Inflame+", "Inflame+", "FlameBarrier+", "FlameBarrier"), enemyCards: Attack(1, 4));

            DoTest("ClearingEnemyBlock", en2: 34, enbl: 10, cis: GetCi("Strike+", "Bash", "Strike"), finalEnemyBlock: 0);
            DoTest("FullBlocked", en2: 50, enbl: 26, cis: GetCi("Strike+", "Bash", "Strike"), finalEnemyBlock: 0);
            DoTest("VulnBreakThrough", en2: 49, enbl: 16, cis: GetCi("Bash", "Strike"), finalEnemyBlock: 0);

            DoTest("double-FlameBarrier-block", pl2: 50, finalPlayerBlock: 24, enbl: 0, en2: 10, cis: GetCi("Inflame+", "Inflame+", "FlameBarrier+", "FlameBarrier"), enemyCards: Attack(1, 4));
            DoTest("Combining statuses works", finalPlayerBlock: 28, cis: GetCi("Inflame+", "Inflame", "FlameBarrier+", "FlameBarrier"),
                playerFinalStatuses: new List<StatusInstance>() {
                    new StatusInstance(new Strength(), 5),
                    new StatusInstance(new FlameBarrierStatus(), 10)});
        }

        public static List<StatusInstance> GetStatuses(Status status, int num)
        {
            return new List<StatusInstance>() { new StatusInstance(status, num) };
        }

        public static List<CardInstance> Attack(int amount, int count)
        {
            return new List<CardInstance>() { new CardInstance(new EnemyAttack(amount, count), 0) };
        }

        public static void RunTests(bool print)
        {
            Helpers.PrintDetails = print;

            DamageBlockTests();
            BasicTests();
            RelicTests();
            PenNibTests();
            EnemyBehaviorTests();

            Console.ReadLine();
        }
    }
}