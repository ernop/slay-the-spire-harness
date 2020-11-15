using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using static StS.AllCards;


namespace StS
{
    public static class Test
    {
        public static Dictionary<string, Card> CardList = GetAllCards();

        public static TestCase CreateTestCase(string name, int pl = 50, int pl2 = 50, int en = 50, int en2 = 50,
            int plbl = 0, int enbl = 0, int finalPlayerblock = 0, int finalEnemyBlock = 0,
            List<Relic> relics = null, List<CardInstance> cis = null, List<CardInstance> enemyCards = null,
            List<StatusInstance> playerStatuses = null, List<StatusInstance> enemyStatuses = null,
            List<StatusInstance> playerFinalStatuses = null,
            List<StatusInstance> enemyFinalStatuses = null)
        {
            var tc = new TestCase()
            {
                EnemyHp = en,
                PlayerBlock = plbl,
                EnemyBlock = enbl,
                FinalEnemyHp = en2,
                PlayerHp = pl,
                FinalPlayerHp = pl2,
                TestName = name,
                CardsToPlay = cis ?? new List<CardInstance>(),
                FinalPlayerBlock = finalPlayerblock,
                FinalEnemyBlock = finalEnemyBlock,
                Relics = relics ?? new List<Relic>(),
                PlayerStatuses = playerStatuses ?? new List<StatusInstance>(),
                EnemyStatuses = enemyStatuses ?? new List<StatusInstance>(),
                EnemyCards = enemyCards ?? new List<CardInstance>(),
                PlayerFinalStatuses = playerFinalStatuses ?? new List<StatusInstance>(),
                EnemyFinalStatuses = enemyFinalStatuses ?? new List<StatusInstance>()
            };
            return tc;
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

        public static void BasicTests(List<TestCase> tcs)
        {
            tcs.Add(CreateTestCase("BashIronwave", en2: 32, cis: GetCi("Bash", "IronWave+"), finalPlayerblock: 7));
            tcs.Add(CreateTestCase("BashInflameIronwave", en2: 27, cis: GetCi("Bash", "Inflame+", "IronWave+"), finalPlayerblock: 7));

            tcs.Add(CreateTestCase("DefendNeg3", cis: GetCi("Footwork", "Defend"), finalPlayerblock: 2, playerStatuses: new List<StatusInstance>() { new StatusInstance(new Dexterity(), int.MaxValue, -5) }));
            tcs.Add(CreateTestCase("DefendNeg5", cis: GetCi("Footwork", "Defend"), finalPlayerblock: 0, playerStatuses: new List<StatusInstance>() { new StatusInstance(new Dexterity(), int.MaxValue, -7) }));
            tcs.Add(CreateTestCase("DefendNeg2", cis: GetCi("Footwork", "Defend"), finalPlayerblock: 3, playerStatuses: new List<StatusInstance>() { new StatusInstance(new Dexterity(), int.MaxValue, -4) }));
            tcs.Add(CreateTestCase("Defend", cis: GetCi("Defend"), finalPlayerblock: 5));
            tcs.Add(CreateTestCase("FootDefend", cis: GetCi("Footwork", "Defend"), finalPlayerblock: 7));
            tcs.Add(CreateTestCase("Foot+Defend", cis: GetCi("Footwork+", "Defend"), finalPlayerblock: 8));
            tcs.Add(CreateTestCase("FootFoot+Defend", cis: GetCi("Footwork", "Footwork+", "Defend"), finalPlayerblock: 10));
            tcs.Add(CreateTestCase("Strike works", en2: 44, cis: GetCi("Strike")));
            tcs.Add(CreateTestCase("Strike, Strike+ works", en2: 35, cis: GetCi("Strike", "Strike+")));


            tcs.Add(CreateTestCase("Bashing", en2: 19, cis: GetCi("Strike", "Bash+", "Strike+")));
            tcs.Add(CreateTestCase("SwordBoomerang", en2: 41, cis: GetCi("SwordBoomerang")));
            tcs.Add(CreateTestCase("SwordBoomerang+", en2: 38, cis: GetCi("SwordBoomerang+")));
            tcs.Add(CreateTestCase("SwordBoomerang+ vs block1", en2: 39, cis: GetCi("SwordBoomerang+"), enbl: 1, finalEnemyBlock: 0));
            tcs.Add(CreateTestCase("SwordBoomerang+ vs block2", en2: 40, cis: GetCi("SwordBoomerang+"), enbl: 2, finalEnemyBlock: 0));
            tcs.Add(CreateTestCase("SwordBoomerang+ vs block3", en2: 41, cis: GetCi("SwordBoomerang+"), enbl: 3, finalEnemyBlock: 0));
            tcs.Add(CreateTestCase("SwordBoomerang+ vs block4", en2: 42, cis: GetCi("SwordBoomerang+"), enbl: 4, finalEnemyBlock: 0));
            tcs.Add(CreateTestCase("SwordBoomerang+ vs block13", cis: GetCi("SwordBoomerang+"), enbl: 13, finalEnemyBlock: 1));
            tcs.Add(CreateTestCase("Inflamed SwordBoomerang+", en2: 30, cis: GetCi("Inflame", "SwordBoomerang+")));

            tcs.Add(CreateTestCase("Inflame", en2: 35, cis: GetCi("Strike", "Inflame+", "Strike")));
            tcs.Add(CreateTestCase("Inflame+Bash", en2: 39, cis: GetCi("Inflame+", "Bash"))); ;
            tcs.Add(CreateTestCase("Inflame+Bash+strike", en2: 26, cis: GetCi("Inflame+", "Bash", "Strike")));
            tcs.Add(CreateTestCase("Inflame+LimitBreak", en2: 32, cis: GetCi("Strike", "Inflame+", "LimitBreak", "Strike")));
            tcs.Add(CreateTestCase("Inflame+LimitBreak+bash", en2: 8, cis: GetCi("Strike", "Inflame+", "LimitBreak", "Bash", "Strike+")));



            tcs.Add(CreateTestCase("WeakPlayer", cis: GetCi("Strike"), en2: 46, playerStatuses: new List<StatusInstance>() { new StatusInstance(new Weak(), 3, int.MaxValue) }));
            tcs.Add(CreateTestCase("WeakenEnemy", cis: GetCi("Uppercut+"), en2: 37,
                enemyFinalStatuses: new List<StatusInstance>() { new StatusInstance(new Vulnerable(), 2, int.MaxValue), new StatusInstance(new Weak(), 2, int.MaxValue) }));
        }

        public static void RelicTests(List<TestCase> tcs)
        {
            var vajraOne = new Vajra { Intensity = 1 };
            tcs.Add(CreateTestCase("Strike, Strike+ works", en2: 33, cis: GetCi("Strike", "Strike+"),
                relics: new List<Relic>() { vajraOne }));

            var torii = new Torii();
            tcs.Add(CreateTestCase("Torii works", pl2: 47,
                relics: new List<Relic>() { torii },
                enemyCards: new List<CardInstance>() {
                new CardInstance(new EnemyAttack(5,3), 0)}));

            tcs.Add(CreateTestCase("Torii Strong Enemy", pl2: 10,
                relics: new List<Relic>() { torii },
                enemyStatuses: new List<StatusInstance>() { new StatusInstance(new Strength(), int.MaxValue, 4) },
                enemyCards: new List<CardInstance>() {
                new CardInstance(new EnemyAttack(6,4),0)}));

            tcs.Add(CreateTestCase("Torii Strong Enemy Weakest attakc", pl2: 46,
                relics: new List<Relic>() { torii },
                enemyStatuses: new List<StatusInstance>() { new StatusInstance(new Strength(), int.MaxValue, 4) },
                enemyCards: new List<CardInstance>() {
                new CardInstance(new EnemyAttack(1,4),0)}));
        }

        public static void PenNibTests(List<TestCase> tcs)
        {
            var penNib = new PenNib();
            penNib.AttackCount = 8;

            tcs.Add(CreateTestCase("PenNib toggles-untoggles", en2: 20, cis: GetCi("Strike", "Strike+", "Strike"), relics: new List<Relic>() { penNib }));

            var penNib2 = new PenNib();
            penNib2.AttackCount = 2;

            tcs.Add(CreateTestCase("PenNib-Nonfunctional", en2: 35, cis: GetCi("Strike", "Strike+"), relics: new List<Relic>() { penNib2 }));


            var penNib3 = new PenNib();
            penNib3.AttackCount = 8;

            tcs.Add(CreateTestCase("PenNib-Inflame", en2: 20, cis: GetCi("Strike", "Inflame+", "Strike+"), relics: new List<Relic>() { penNib3 }));

            var penNib4 = new PenNib();
            penNib4.AttackCount = 8;
            tcs.Add(CreateTestCase("Inflamed Nibbed SwordBoomerang+", en2: 4, cis: GetCi("Strike", "Inflame", "SwordBoomerang+"), relics: new List<Relic>() { penNib4 }));
        }

        public static void EnemyBehaviorTests(List<TestCase> tcs)
        {
            tcs.Add(CreateTestCase("Enemy-attacks-disarmed", pl2: 43, en2: 41, cis: GetCi("Strike+", "Disarm+"), enemyCards: new List<CardInstance>() { new CardInstance(new EnemyAttack(10, 1), 0) }));

            tcs.Add(CreateTestCase("Enemy-attacks-bash", pl2: 20, en2: 41, cis: GetCi("Strike+"), enemyCards: new List<CardInstance>() {
                new CardInstance(new EnemyAttack(10, 2), 0) },
                playerStatuses: new List<StatusInstance>() { new StatusInstance(new Vulnerable(), 3, int.MaxValue) }));

            var si = new List<StatusInstance>() { new StatusInstance(new Aggressive(), int.MaxValue, 4) };
            tcs.Add(CreateTestCase("Louse-Aggressive", en2: 41, cis: GetCi("Strike+"), enemyStatuses: si, finalEnemyBlock: 4));

            var si2 = new List<StatusInstance>() { new StatusInstance(new Aggressive(), int.MaxValue, 4) };
            tcs.Add(CreateTestCase("Louse-Aggressive-triggered-cleared", en2: 33, cis: GetCi("Strike+", "Inflame+", "LimitBreak", "Strike"), enemyStatuses: si2, finalEnemyBlock: 0));

            tcs.Add(CreateTestCase("Enemy-attacks", pl2: 40, en2: 41, cis: GetCi("Strike+"), enemyCards: new List<CardInstance>() { new CardInstance(new EnemyAttack(10, 1), 0) }));


        }

        public static void DamageBlockTests(List<TestCase> tcs)
        {
            tcs.Add(CreateTestCase("ClearingEnemyBlock", en2: 34, enbl: 10, cis: GetCi("Strike+", "Bash", "Strike"), finalEnemyBlock: 0));
            tcs.Add(CreateTestCase("FullBlocked", en2: 50, enbl: 26, cis: GetCi("Strike+", "Bash", "Strike"), finalEnemyBlock: 0));
            tcs.Add(CreateTestCase("VulnBreakThrough", en2: 49, enbl: 16, cis: GetCi("Bash", "Strike"), finalEnemyBlock: 0));
        }

        public static void RunTests(bool print)
        {
            Helpers.PrintDetails = print;

            var tcs = new List<TestCase>();

            if (true)
            {
                BasicTests(tcs);
                DamageBlockTests(tcs);

                RelicTests(tcs);
                PenNibTests(tcs);
                EnemyBehaviorTests(tcs);
            }

            foreach (var testCase in tcs)
            {
                testCase.Run();
            }

            Console.ReadLine();
        }
    }
}