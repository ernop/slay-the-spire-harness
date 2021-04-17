using System.Collections.Generic;

namespace StS
{
    public class Immolate : IroncladAttackCard
    {
        public override string Name => nameof(Immolate);

        public override EnergyCostInt CiCanCallEnergyCost(int upgradeCount) => new EnergyCostInt(2);

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            var dmg = upgradeCount == 0 ? 21 : 28;
            ef.EnemyEffect.SetInitialDamage(dmg);
            ef.DeckEffect.Add((Deck d, List<string> h) =>
            {
                var burn = new CardInstance(new Burn(), 0);
                d.AddToDiscardPile(burn);
                h.Add("Added burn to discard pile");
            });
        }
    }
}
