using System.Collections.Generic;

namespace StS
{
    public class FeelNoPain : PowerCard
    {
        public override string Name => nameof(FeelNoPain);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override int CiCanCallEnergyCost(int upgradeCount) => 1;

        internal override void Play(EffectSet ef, IEntity source, IEntity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            var amt = upgradeCount == 0 ? 3 : 4;
            ef.TargetEffect.Status.Add(new StatusInstance(new FeelNoPainStatus(), amt));
        }
    }
}
