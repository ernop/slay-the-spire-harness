
using System;

using static StS.Helpers;

namespace StS
{
    class Program
    {
        static void Main(string[] args)
        {
            Helpers.SetRandom(0);
            var cis = GetCis("Strike", "Strike", "Strike", "Strike", "Strike", "Bash", "Defend", "Defend", "Defend", "Defend", "Defend");

            var enemy = new Cultist(hp: 10);
            var player = new Player(relics: GetRelics("BurningBlood"));
            var fs = new FightSimulator(cis, enemy, player);
            var res = fs.Sim();
            foreach (var r in res)
            {
                Console.WriteLine("==============");
                foreach (var h in r.FightHistory)
                {
                    Console.WriteLine(h);
                }
            }
        }
    }
}