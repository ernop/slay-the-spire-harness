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

        public static TestCase CreateTestCase(string name, int pl, int pl2, int en, int en2, List<CardInstance> cardInstances, int playerBlock = 0, List<Relic> relics = null)
        {
            var tc = new TestCase()
            {
                EnemyHP = en,
                PostCardsEnemyHp = en2,
                PlayerHP = pl,
                PostCardsPlayerHp = pl2,
                TestName = name,
                CardsToPlay = cardInstances,
                PlayerBlock = playerBlock,
                Relics = relics
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
            tcs.Add(CreateTestCase("Defend", 50, 50, 50, 50, GetCi("Defend"), 5));
            tcs.Add(CreateTestCase("FootDefend", 50, 50, 50, 50, GetCi("Footwork", "Defend"), 7));
            tcs.Add(CreateTestCase("Foot+Defend", 50, 50, 50, 50, GetCi("Footwork+", "Defend"), 8));
            tcs.Add(CreateTestCase("FootFoot+Defend", 50, 50, 50, 50, GetCi("Footwork", "Footwork+", "Defend"), 10));
            tcs.Add(CreateTestCase("Strike works", 50, 50, 50, 44, GetCi("Strike")));
            tcs.Add(CreateTestCase("Strike, Strike+ works", 50, 50, 50, 35, GetCi("Strike", "Strike+")));


            tcs.Add(CreateTestCase("Bashing", 50, 50, 50, 19, GetCi("Strike", "Bash+", "Strike+")));
            tcs.Add(CreateTestCase("Inflame", 50, 50, 50, 35, GetCi("Strike", "Inflame+", "Strike")));
            tcs.Add(CreateTestCase("Inflame+Bash", 50, 50, 50, 39, GetCi("Inflame+", "Bash")));
            tcs.Add(CreateTestCase("Inflame+Bash+strike", 50, 50, 50, 26, GetCi("Inflame+", "Bash", "Strike")));
            tcs.Add(CreateTestCase("Inflame+LimitBreak", 50, 50, 50, 32, GetCi("Strike", "Inflame+", "LimitBreak", "Strike")));
            tcs.Add(CreateTestCase("Inflame+LimitBreak+bash", 50, 50, 50, 8, GetCi("Strike", "Inflame+", "LimitBreak", "Bash", "Strike+")));
        }

        public static void RelicTests(List<TestCase> tcs)
        {
            var varjaOne = new Varja { Intensity = 1 };
            tcs.Add(CreateTestCase("Strike, Strike+ works", 50, 50, 50, 33, GetCi("Strike", "Strike+"), 0,
                new List<Relic>() { varjaOne }));
        }

        public static void PenNibTests(List<TestCase> tcs)
        {
            var penNib = new PenNib();
            penNib.AttackCount = 8;

            tcs.Add(CreateTestCase("PenNib toggles-untoggles", 50, 50, 50, 20, GetCi("Strike", "Strike+", "Strike"), 0, new List<Relic>() { penNib }));

            var penNib2 = new PenNib();
            penNib2.AttackCount = 2;

            tcs.Add(CreateTestCase("PenNib-Nonfunctional", 50, 50, 50, 35, GetCi("Strike", "Strike+"), 0, new List<Relic>() { penNib2 }));


            var penNib3 = new PenNib();
            penNib3.AttackCount = 8;

            tcs.Add(CreateTestCase("PenNib-Inflame", 50, 50, 50, 20, GetCi("Strike", "Inflame+", "Strike+"), 0, new List<Relic>() { penNib3 }));
        }

        public static void RunTests(bool print)
        {
            Helpers.PrintDetails = print;

            var tcs = new List<TestCase>();

            if (true)
            {
                BasicTests(tcs);
                RelicTests(tcs);
                PenNibTests(tcs);                
            }

            foreach (var testCase in tcs)
            {
                testCase.Run();
            }

            Console.ReadLine();
        }
    }
}