using System.Collections.Generic;

namespace StS
{
    public class PommelStrike : IroncladAttackCard
    {
        public override string Name => nameof(PommelStrike);

        public override int CiCanCallEnergyCost(int upgradeCount) => 1;

        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount, List<CardInstance> targetCards = null, Deck deck = null)
        {
            var cardAmt = upgradeCount == 0 ? 1 : 2;
            var dmg = upgradeCount == 0 ? 9 : 10;
            ef.TargetEffect.InitialDamage = new List<int>() { dmg };
            ef.DeckEffect.Add((Deck d) =>
            {
                d.DrawToHand(this, targetCards, cardAmt, true);
            });
        }
    }
}
