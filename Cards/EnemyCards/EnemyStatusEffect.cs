
using System.Collections.Generic;

namespace StS
{
    public class EnemyStatusEffect : EnemyCard
    {
        public override string Name => nameof(EnemyStatusEffect);
        public override CardType CardType => CardType.Skill;

        public override TargetType TargetType => TargetType.Player;

        public override int CiCanCallEnergyCost(int upgradeCount) => 0;

        private int Num { get; set; }
        private StatusInstance Si { get; set; }
        public EnemyStatusEffect(StatusInstance si)
        {
            Si = si;
        }

        internal override void Play(EffectSet ef, IEntity source, IEntity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            ef.TargetEffect.Status.Add(Si);
        }
    }
}
