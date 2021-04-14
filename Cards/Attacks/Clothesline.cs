using System.Collections.Generic;

namespace StS
{

    public class Clothesline : IroncladAttackCard
    {
        public override string Name => nameof(Clothesline);

        public override int CiCanCallEnergyCost(int upgradeCount) => 2;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            int dmg;
            StatusInstance si;
            if (upgradeCount == 0)
            {
                dmg = 12;
                si = new StatusInstance(new Weak(), 2);
            }
            else
            {
                dmg = 14;
                si = new StatusInstance(new Weak(), 3);
            }
            ef.EnemyEffect.SetInitialDamage(dmg);
            ef.EnemyEffect.Status.Add(si);
        }
    }
}
