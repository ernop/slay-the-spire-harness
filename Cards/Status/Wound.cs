using System.Collections.Generic;

namespace StS
{
    public class Wound : StatusCard
    {
        public override string Name => nameof(Wound);

        public override CharacterType CharacterType => CharacterType.Colorless;

        public override TargetType TargetType => TargetType.Player;

        public override int CiCanCallEnergyCost(int upgradeCount) => int.MaxValue;

        internal override void Play(EffectSet ef, IEntity source, IEntity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            throw new System.Exception("Cannot play");
        }
    }
}
