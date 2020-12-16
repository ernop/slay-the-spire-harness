using System.Linq;

namespace StS
{

    public class Torii : Relic
    {
        public override string Name => nameof(Torii);
        internal override Relic Copy() => new Torii();

        public override void Apply(Fight f, Deck d, Player p)
        {
            p.BeAttacked += AttackResponse;
        }

        public override void Unapply(Fight f, Deck d, Player p)
        {
            p.BeAttacked += AttackResponse;
        }

        private void AttackResponse(EffectSet ef)
        {
            //triggers when player with this status is targeted.

            //we grab the progression from the damage pattern I'd receive, with default values of zero.
            //and this must be an enemy action.

            ef.PlayerEffect.DamageAdjustments.Add(
                        new AttackProgression("ToriiReduction", (el) => el.Select(qq => qq <= 5 && qq > 0 ? 1 : qq).ToList()));
        }
    }
}
