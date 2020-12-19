
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

            var enemy = new Cultist(hp: 51, hpMax: 51);
            var player = new Player(hp: 51);
            var fs = new FightSimulator(cis, enemy, player, doOutput: true, oneStartingHandOnly: true, depth: 5);
            var node = fs.Sim();

            node.Display(Helpers.Output);
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