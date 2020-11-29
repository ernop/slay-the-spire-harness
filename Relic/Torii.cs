using System.Linq;

namespace StS
{

    public class Torii : Relic
    {
        public override string Name => nameof(Torii);
        internal override Relic Copy() => new Torii();
        public override void CardPlayed(Card card, EffectSet ef, IEntity player, IEntity enemy)
        {
            if (card.CardType == CardType.Attack)
            {
                if (ef.TargetEffect.DamageAdjustments != null)
                {
                    ef.TargetEffect.DamageAdjustments.Add(
                        new AttackProgression("ToriiReduction", (el) => el.Select(qq => qq <= 5 && qq > 0 ? 1 : qq).ToList()));
                }
            }
        }
    }
}
