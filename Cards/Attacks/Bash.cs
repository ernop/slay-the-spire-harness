using System.Collections.Generic;

namespace StS
{
    public class Bash : Card
    {
        public override string Name => "Bash";
        public override CharacterType CharacterType => CharacterType.IronClad;
        public override CardType CardType => CardType.Attack;
        public override void Apply(Player player, Enemy enemy, List<Enemy> enemyList, int upgradeCount)
        {
            int amt;
            StatusInstance si;
            if (upgradeCount == 0)
            {
                amt = 8;
                si = new StatusInstance(new Vulnerable(), 2, int.MaxValue);
            }
            else
            {
                amt = 12;
                si = new StatusInstance(new Vulnerable(), 3, int.MaxValue); 
            }

            amt = player.AdjustDealtDamage(amt);
            
            enemy.ApplyDamage(amt);
            enemy.ApplyStatus(si);
        }
    }
}
