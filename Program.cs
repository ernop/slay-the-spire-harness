﻿
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

            TestAttacker();

        }

        static void TestCultist()
        {
            var cis = GetCis("Strike", "Defend", "Inflame", "Carnage", "Disarm", "Thunderclap", "IronWave+");

            var enemy = new Cultist(hp: 1);
            var player = new Player();
            var fs = new FightSimulator(cis, enemy, player);
            var res = fs.Sim();
            foreach (var rootNode in res)
            {
                rootNode.Display(_Output);
            }
        }

        static void TestAttacker()
        {
            var cis = GetCis("Pummel+", "Inflame", "Inflame", "Inflame", "Defend");

            var enemy = new GenericEnemy(5, 1, 50, 50);
            var player = new Player(potions: new List<Potion>() { new StrengthPotion() });
            var fs = new FightSimulator(cis, enemy, player, true);
            var res = fs.Sim();
            foreach (var rootNode in res)
            {
                rootNode.Display(_Output);
            }
        }
    }
}