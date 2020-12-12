using System.Linq;

namespace StS
{
    public class EvolveStatus : Status
    {
        public override string Name => nameof(EvolveStatus);

        public override StatusType StatusType => StatusType.EvolveStatus;

        public override bool NegativeStatus => false;

        internal override bool Scalable => true;

        internal override bool Permanent => true;
        private Entity Entity { get; set; }

        public override void Apply(Deck d, Entity e)
        {
            d.DrawCard += EvolveCardDrawnEvent;
            Entity = e;
        }

        public override void Unapply(Deck d, Entity e)
        {
            d.DrawCard -= EvolveCardDrawnEvent;
            Entity = null;
        }

        internal void EvolveCardDrawnEvent(CardInstance ci, EffectSet ef)
        {
            var si = Entity.StatusInstances.SingleOrDefault(el => el.Status.StatusType == StatusType.EvolveStatus);
            if (si == null)
            {
                return;
            }
            if (ci.Card.CardType == CardType.Curse || ci.Card.CardType == CardType.Status)
            {
                var newEf = new EffectSet();
                newEf.DeckEffect.Add((Deck d) =>
                {
                    var drawn = d.DrawToHand(null, si.Intensity, true, newEf);
                    return $"{ci} Caused card draw: {string.Join(',', drawn)}";
                });
                ef.AddNextEf(newEf);
            }
        }
    }
}
