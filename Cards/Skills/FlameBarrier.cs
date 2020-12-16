using System.Collections.Generic;

namespace StS
{
    public class FlameBarrier : IroncladSkillCard
    {
        public override string Name => nameof(FlameBarrier);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override TargetType TargetType => TargetType.Player;
        public override int CiCanCallEnergyCost(int upgradeCount) => 2;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            var blockAmount = upgradeCount == 0 ? 12 : 16;
            ef.PlayerEffect.InitialBlock = blockAmount;

            var intensity = upgradeCount == 0 ? 4 : 6;
            ef.PlayerEffect.Status.Add(new StatusInstance(new FlameBarrierStatus(), intensity));
        }
    }
}
