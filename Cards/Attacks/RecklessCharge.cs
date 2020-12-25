using System.Collections.Generic;

namespace StS
{
    public class RecklessCharge : IroncladAttackCard
    {
        public override string Name => nameof(RecklessCharge);

        public override int CiCanCallEnergyCost(int upgradeCount) => 0;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null)
        {
            var dmg = upgradeCount == 0 ? 7 : 10;
            ef.DeckEffect.Add((Deck d, List<string> h) =>
            {
                var dazed = new CardInstance(new Dazed(), 0);
                d.AddToDrawPile(dazed);
                return "Reckless charge added dazed to draw";
            });
            ef.EnemyEffect.SetInitialDamage(dmg);
        }
    }
}
