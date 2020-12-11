
using System.Collections.Generic;

using static StS.Helpers;

namespace StS
{
    class Program
    {
        static string _Output = "C:/dl/output.txt";
        static void Main(string[] args)
        {
            Helpers.SetRandom(0);
            //TestSimple();

        }

        

        static void TestCultist()
        {
            var cis = GetCis("Strike", "Defend", "Inflame", "Carnage", "Disarm", "Thunderclap", "IronWave+");

            var enemy = new Cultist(hp: 1);
            var player = new Player();
            var fs = new FightSimulator(cis, enemy, player);
            var node = fs.Sim();

            node.DisplayFirstRound(_Output);

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
                System.IO.File.AppendAllText(_Output, "==One draw.");
                draw.DisplayFirstRound(_Output);
            }

            fs.SaveResults(_Output, node);
        }
    }
}