
using static StS.Helpers;

namespace StS
{
    class Program
    {
        static void Main(string[] args)
        {
            var cis = GetCis("Strike", "Strike", "Strike", "Strike", "Strike", "Bash", "Defend", "Defend", "Defend", "Defend", "Defend");

            var enemy = new Cultist();
            var player = new Player(relics: GetRelics("BurningBlood"));
            var fs = new FightSimulator(cis, enemy, player);
            fs.Sim();
        }
    }
}