
using System;

using static StS.Helpers;

namespace StS
{
    public class SimTests
    {
        //[Test]
        public void Test_Basic()
        {
            //I want to validate that I am properly generating all outcomes.
            var cis = GetCis("Strike", "Defend");

            var enemy = new Cultist(5, 5);
            var player = new Player();
            var fs = new FightSimulator(cis, enemy, player);
            var results = fs.Sim();
            Console.WriteLine("==========New Fight===========");

            var ii = 0;
            foreach (var fight in results)
            {
                foreach (var h in fight.FightHistory)
                {
                    Console.WriteLine(h);
                }
                ii++;
                if (ii > 10)
                {
                    break;
                }
            }
        }
    }
}