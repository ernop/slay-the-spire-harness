
using System.Collections.Generic;

using static StS.Helpers;

namespace StS
{
    class Program
    {
        static void Main(string[] args)
        {
            Helpers.SetRandom(0);
            //TestSimple();
            TestCultist();

        }



        static void TestCultist()
        {
            var cis = GetCis("Strike", "Defend", "Inflame", "Carnage", "Disarm", "Thunderclap", "IronWave+");

            var enemy = new Cultist(hp: 1);
            var player = new Player();
            var fs = new FightSimulator(cis, enemy, player, doOutput: true);
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