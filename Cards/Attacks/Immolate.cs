using System.Collections.Generic;

namespace StS
{
    public class Immolate : IroncladAttackCard
    {
        public override string Name => nameof(Immolate);

        public override int CiCanCallEnergyCost(int upgradeCount) => 2;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            var dmg = upgradeCount == 0 ? 21 : 28;
            ef.EnemyEffect.SetInitialDamage(dmg);
            ef.DeckEffect.Add((Deck d) =>
            {
                var burn = new CardInstance(new Burn(), 0);
                d.AddToDiscardPile(burn);
                return "Added burn to discard pile";
            });
        }
    }
}
