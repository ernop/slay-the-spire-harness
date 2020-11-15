using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using static StS.AllCards;


namespace StS 
{
    public static class Test
    {
        public static Dictionary<string,Card> CardList = GetAllCards();

        public static TestCase CreateTestCase(string name, int pl = 50, int pl2 = 50, int en = 50, int en2 = 50,
            int plbl = 0, int enbl = 0, int finalPlayerblock = 0, int finalEnemyBlock = 0,
            List<Relic> relics = null, List<CardInstance> cis = null, List<CardInstance> enemyCards = null,
            List<StatusInstance> playerStatuses = null, List < StatusInstance> enemyStatuses = null)
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
                EnemyCards = enemyCards ?? new List<CardInstance>()
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
            //tcs.Add(CreateTestCase("DefendNeg3", cis: GetCi("Footwork","Defend"), finalPlayerblock: 2, playerStatuses:new List<StatusInstance>() { new StatusInstance(new Dexterity(), int.MaxValue, -5) }));
            tcs.Add(CreateTestCase("DefendNeg5", cis: GetCi("Footwork", "Defend"), finalPlayerblock: 0, playerStatuses: new List<StatusInstance>() { new StatusInstance(new Dexterity(), int.MaxValue, -7) }));
            tcs.Add(CreateTestCase("DefendNeg2", cis: GetCi("Footwork", "Defend"), finalPlayerblock: 3, playerStatuses: new List<StatusInstance>() { new StatusInstance(new Dexterity(), int.MaxValue, -4) }));
            tcs.Add(CreateTestCase("Defend", cis: GetCi("Defend"), finalPlayerblock: 5));
            tcs.Add(CreateTestCase("FootDefend", cis: GetCi("Footwork", "Defend"), finalPlayerblock: 7));
            tcs.Add(CreateTestCase("Foot+Defend", cis: GetCi("Footwork+", "Defend"), finalPlayerblock: 8));
            tcs.Add(CreateTestCase("FootFoot+Defend", cis: GetCi("Footwork", "Footwork+", "Defend"), finalPlayerblock: 10));
            tcs.Add(CreateTestCase("Strike works", en2:44, cis: GetCi("Strike")));
            tcs.Add(CreateTestCase("Strike, Strike+ works", en2: 35, cis: GetCi("Strike", "Strike+")));


            tcs.Add(CreateTestCase("Bashing", en2: 19, cis: GetCi("Strike", "Bash+", "Strike+")));
            tcs.Add(CreateTestCase("Inflame", en2: 35, cis: GetCi("Strike", "Inflame+", "Strike")));
            tcs.Add(CreateTestCase("Inflame+Bash", en2: 39, cis: GetCi("Inflame+", "Bash"))); ;
            tcs.Add(CreateTestCase("Inflame+Bash+strike", en2: 26, cis: GetCi("Inflame+", "Bash", "Strike")));
            tcs.Add(CreateTestCase("Inflame+LimitBreak", en2: 32, cis: GetCi("Strike", "Inflame+", "LimitBreak", "Strike")));
            tcs.Add(CreateTestCase("Inflame+LimitBreak+bash", en2: 8, cis: GetCi("Strike", "Inflame+", "LimitBreak", "Bash", "Strike+")));

            tcs.Add(CreateTestCase("BashIronwave", en2: 32, cis: GetCi("Bash", "IronWave+"), finalPlayerblock : 7));
            tcs.Add(CreateTestCase("BashInflameIronwave", en2:27, cis: GetCi("Bash", "Inflame+", "IronWave+"), finalPlayerblock: 7));
        }

        public static void RelicTests(List<TestCase> tcs)
        {
            var varjaOne = new Varja { Intensity = 1 };
            tcs.Add(CreateTestCase("Strike, Strike+ works", en2: 33, cis: GetCi("Strike", "Strike+"),
                relics: new List<Relic>() { varjaOne }));
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
        }

        public static void EnemyBehaviorTests(List<TestCase> tcs)
        {
            tcs.Add(CreateTestCase("Enemy-attacks-disarmed", pl2: 43, en2: 41, cis: GetCi("Strike+", "Disarm+"), enemyCards: new List<CardInstance>() { new CardInstance(new EnemyAttack(10), 0) }));

            tcs.Add(CreateTestCase("Enemy-attacks-bash", pl2: 20, en2: 41, cis: GetCi("Strike+"), enemyCards: new List<CardInstance>() {
                new CardInstance(new EnemyAttack(10), 0),
                new CardInstance(new EnemyAttack(10), 0)},
                playerStatuses: new List<StatusInstance>() { new StatusInstance(new Vulnerable(), 3, int.MaxValue) }));

            var si = new List<StatusInstance>() { new StatusInstance(new Aggressive(), int.MaxValue, 4) };
            tcs.Add(CreateTestCase("Louse-Aggressive", en2: 41, cis: GetCi("Strike+"), enemyStatuses: si, finalEnemyBlock: 4));

            var si2 = new List<StatusInstance>() { new StatusInstance(new Aggressive(), int.MaxValue, 4) };
            tcs.Add(CreateTestCase("Louse-Aggressive-triggered-cleared", en2: 33, cis: GetCi("Strike+", "Inflame+", "LimitBreak", "Strike"), enemyStatuses: si2, finalEnemyBlock: 0));

            tcs.Add(CreateTestCase("Enemy-attacks", pl2: 40, en2: 41, cis: GetCi("Strike+"), enemyCards: new List<CardInstance>() { new CardInstance(new EnemyAttack(10), 0) }));
            
            
        }

        public static void DamageBlockTests(List<TestCase> tcs)
        {
            tcs.Add(CreateTestCase("ClearingEnemyBlock", en2: 34, enbl:10, cis: GetCi("Strike+", "Bash", "Strike"), finalEnemyBlock: 0));
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