using System.Linq;

namespace StS
{
    /// <summary>
    /// when created linked to a player, it will subscribe to deck.exhaust so that whenever the deck exhausts, we just directly add block to the player.
    /// </summary>
    public class FeelNoPainStatus : Status
    {
        public override string Name => nameof(FeelNoPainStatus);

        public override StatusType StatusType => StatusType.FeelNoPainStatus;

        public override bool NegativeStatus => false;

        internal override bool Scalable => true;

        internal override bool Permanent => true;

        private IEntity Entity { get; set; }

        public override void Apply(Deck d, Entity e)
        {
            d.ExhaustCard += FeelNoPainStatusResponseToExhaustion;
            Entity = e;
        }

        public override void Unapply(Deck d, Entity e)
        {
            d.ExhaustCard -= FeelNoPainStatusResponseToExhaustion;
            Entity = null;
        }

        private void FeelNoPainStatusResponseToExhaustion(EffectSet ef)
        {
            //for some reason we look up the player's SI again even though we 
            var si = Entity.StatusInstances.SingleOrDefault(el => el.Status.StatusType == StatusType.FeelNoPainStatus);
            if (si == null)
            {
                return;
            }
            //can we just set player block? No never.  Because there are things like "do damage when gain block".
            ef.TargetEffect.BlockAdjustments.Add(new Progression("FPN Blocks", (el, entity) =>
            {
                return el + si.Intensity;
            }));
        }
    }
}
