
using System.Collections.Generic;
using System.Linq;
using static StS.Helpers;

namespace StS
{
    class Program
    {

        static List<CardInstance> InitialDeck = GetCis("Strike", "Strike", "Strike", "Defend", "Bash", "Strike", "Strike", "Defend", "Defend", "Defend").ToList();

        static void Main(string[] args)
        {
            SetRandom(0);
            System.IO.File.WriteAllText(Helpers.Output, "");
            var c = new StsConsole();
            //TestCultistMC();
        }

        static void TestCultistMC()
        {
            var turnNumber = 0;
            
            var initialDeck = new List<string>() { "Strike", "Strike", "Strike", "Strike", "Strike", "Defend", "Defend", "Defend", "Defend", "Bash" };
            //initialHand.AddRange(new List<string>() {"Pummel", "Exhume"});

            var hand = Gsl();
            var drawPile = initialDeck;
            var exhaustPile = Gsl();
            var discardPile = Gsl();
            var enemyHp = 51;
            var playerHp = 1;
            var firstDraw = new List<string>() { "Strike", "Strike", "Defend", "Defend", "Bash" };
            var statuses = new List<StatusInstance>() {  };

            if (false)
            {
                turnNumber = 1;
                discardPile = firstDraw;
                drawPile = Gsl("Strike", "Strike", "Defend", "Defend", "Strike");
                firstDraw = Gsl("Strike", "Strike", "Defend", "Defend", "Strike");
                statuses = new List<StatusInstance>() { GS(new Feather(), 3), GS(new Vulnerable(), 1) };
            }

            var enemy = new Cultist(hp: enemyHp);
            enemy.StatusInstances = statuses;
            var player = new Player(hp: playerHp);

            var deck = new Deck(drawPile, hand, discardPile, exhaustPile);
            
            var mc = new MonteCarlo(deck, enemy, player, turnNumber, firstDraw);
            var bestValue = new NodeValue(-100,0, null);

            for (var ii = 0; ii < 100000; ii++)
            {
                var leaf = mc.MC(mc.Root.Randoms.First());
                var value = leaf.GetValue();
                if (value > bestValue)
                {
                    bestValue = value;
                    SaveLeaf(leaf, ii);
                    ShowInitialValues(mc.Root.Randoms.First());
                }
            }

            System.IO.File.AppendAllText(Output, $"==========Final First move best lines");
            ShowInitialValues(mc.Root.Randoms.First());
            SaveAllLeaves(mc.Root);
        }

        public static void SaveAllLeaves(FightNode root)
        {
            var leaves = GetAllLeaves(root);
            var ii = 0;
            foreach (var l in leaves)
            {
                ii++;
                SaveLeaf(l, ii);
                if (ii > 10000)
                {
                    break;
                }
            }
        }

        public static List<FightNode> GetAllLeaves(FightNode root)
        {
            var res = new List<FightNode>();
            var leaf = true;
            foreach (var c in root.Choices)
            {
                res.AddRange(GetAllLeaves(c));
                leaf = false;
            }
            foreach (var r in root.Randoms)
            {
                res.AddRange(GetAllLeaves(r));
                leaf = false;
            }
            if (leaf)
            {
                res.Add(root);
            }

            return res;
        }

        public static void ShowInitialValues(FightNode drawNode)
        {
            System.IO.File.AppendAllText(Output, $"\n---------Best FirstRound Choices: {drawNode.Choices.Count}");
            foreach (var c in drawNode.Choices.OrderByDescending(el=>el.Value))
            {
                ShowRound(c);
            }
        }

        public static void ShowRound(FightNode c)
        {
            var target = c;
            var res = new List<string>();
            res.Add($"{c.Value.ToString()} W:{c.Weight}");
            while (true)
            {
                res.Add(target.ToString());
                if (target.Value.BestChoice == null)
                {
                    break;
                }
                target = target.Value.BestChoice;
            }

            System.IO.File.AppendAllText(Output, "\n");
            System.IO.File.AppendAllLines(Output, res);
        }

        static void SaveLeaf(FightNode leaf, int mcCount)
        {
            System.IO.File.AppendAllText(Output, $"\n==============Fight {mcCount} {leaf.Fight.Status} {leaf.Value}\n");
            var hh = leaf.AALeafHistory();
            System.IO.File.AppendAllText(Output, SJ(hh, '\n'));
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
    }
}