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

        public static TestCase CreateTestCase(string name, int pl, int pl2, int en, int en2, List<CardInstance> cardInstances, int playerBlock = 0)
        {
            var tc = new TestCase()
            {
                EnemyHP = en,
                PostCardsEnemyHp = en2,
                PlayerHP = pl,
                PostCardsPlayerHp = pl2,
                TestName = name,
                CardsToPlay = cardInstances,
                PlayerBlock = playerBlock
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

        public static void RunTests(bool print)
        {
            Helpers.PrintDetails = print;

            
            var tcs = new List<TestCase>();
            tcs.Add(CreateTestCase("Defend", 50, 50, 50, 50, GetCi("Defend"), 5));
            tcs.Add(CreateTestCase("FootDefend", 50, 50, 50, 50, GetCi("Footwork", "Defend"), 7));
            tcs.Add(CreateTestCase("Foot+Defend", 50, 50, 50, 50, GetCi("Footwork+", "Defend"), 8));
            tcs.Add(CreateTestCase("FootFoot+Defend", 50, 50, 50, 50, GetCi("Footwork", "Footwork+", "Defend"), 10));
            tcs.Add(CreateTestCase("Strike works", 50, 50, 50, 44, GetCi("Strike")));
            tcs.Add(CreateTestCase("Strike, Strike+ works", 50, 50, 50, 35, GetCi("Strike", "Strike+")));
            tcs.Add(CreateTestCase("Bashing", 50, 50, 50, 19, GetCi("Strike", "Bash+", "Strike+")));
            tcs.Add(CreateTestCase("Inflamer", 50, 50, 50, 35, GetCi("Strike", "Inflame+", "Strike")));
            tcs.Add(CreateTestCase("Inflamer+Bash", 50, 50, 50, 39, GetCi("Inflame+", "Bash")));
            tcs.Add(CreateTestCase("Inflamer+Bash+strike", 50, 50, 50, 26, GetCi("Inflame+", "Bash", "Strike")));
            tcs.Add(CreateTestCase("Inflamer+LimitBreak", 50, 50, 50, 32, GetCi("Strike", "Inflame+", "LimitBreak", "Strike")));
            tcs.Add(CreateTestCase("Inflamer+LimitBreak+bash", 50, 50, 50, 8, GetCi("Strike", "Inflame+", "LimitBreak", "Bash", "Strike+")));

            foreach (var testCase in tcs)
            {
                testCase.Run();
            }

            Console.ReadLine();
        }
    }
}