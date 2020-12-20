using System.Collections.Generic;

namespace StS
{
    public class PommelStrike : IroncladAttackCard
    {
        public override string Name => nameof(PommelStrike);

        public override int CiCanCallEnergyCost(int upgradeCount) => 1;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            var cardAmt = upgradeCount == 0 ? 1 : 2;
            var dmg = upgradeCount == 0 ? 9 : 10;
            ef.EnemyEffect.SetInitialDamage(dmg);

            ef.DeckEffect.Add((Deck d) =>
            {
                var drawn = d.DrawToHand(targets, cardAmt, true, ef);
                return $"PommelstrikeDrew {string.Join(',', drawn)}";
            });
        }
    }
}
