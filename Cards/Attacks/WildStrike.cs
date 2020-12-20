using System.Collections.Generic;

namespace StS
{
    public class WildStrike : IroncladAttackCard
    {
        public override string Name => nameof(WildStrike);

        public override int CiCanCallEnergyCost(int upgradeCount) => 1;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            ef.DeckEffect.Add((Deck d) =>
            {
                var newCi = new CardInstance(new Wound(), 0);
                d.AddToDrawPile(newCi);
                return "Wound added to draw pile";
            });
            var dmg = upgradeCount == 0 ? 12 : 17;
            ef.EnemyEffect.SetInitialDamage(dmg);
        }
    }
}
