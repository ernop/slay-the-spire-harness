
using System.Collections.Generic;

using static StS.Helpers;

namespace StS
{
    class Program
    {

        static List<CardInstance> InitialHand = GetCis("Strike", "Strike", "Strike", "Defend", "Bash", "Strike", "Strike", "Defend", "Defend", "Defend");

        static void Main(string[] args)
        {
            Helpers.SetRandom(0);
            //TestSimple();
            TestCultist();
        }

        static void TestCultist()
        {
            var cis = InitialHand;
            cis = GetCis("Strike");
            var enemy = new Cultist(hp: 10, hpMax: 10);
            var player = new Player(hp: 51);
            var fs = new FightSimulator(cis, enemy, player, doOutput: true, oneStartingHandOnly: true, depth: 10);
            var node = fs.Sim();
            var leaves = GetLeaves(node);
            foreach (var l in leaves)
            {
                var fh = l.AALeafHistory();

                System.IO.File.AppendAllText(Helpers.Output, "==============\n");
                System.IO.File.AppendAllLines(Helpers.Output, fh);
            }

            //node.Display(Helpers.Output);
        }

        public static List<FightNode> GetLeaves(FightNode node)
        {
            var res = new List<FightNode>();
            foreach (var set in new List<List<FightNode>> { node.Choices, node.Randoms })
            {
                foreach (var c in set)
                {
                    if (c.Fight.Status != FightStatus.Ongoing)
                    {
                        res.Add(c);
                    }
                    else
                    {
                        res.AddRange(GetLeaves(c));
                    }
                }
            }
            return res;
        }

        static void TestAttacker()
        {
            var cis = GetCis("Defend", "Defend", "Defend", "Defend", "Defend", "Strike", "Strike", "Strike", "Strike", "Strike");

            var enemy = new GenericEnemy(20, 1, 50, 50);
            var player = new Player(potions: new List<Potion>() { new StrengthPotion() });
            var fs = new FightSimulator(cis, enemy, player, true);
            var node = fs.Sim();

            foreach (var draw in node.Randoms)
            {
                System.IO.File.AppendAllText(Output, "==One draw.");
                draw.Display(Output);
            }

            fs.SaveResults(Output, node);
        }
    }
}