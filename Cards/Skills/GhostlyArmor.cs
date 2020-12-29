using System.Collections.Generic;

namespace StS
{
    public class GhostlyArmor : IroncladSkillCard
    {
        public override string Name => nameof(GhostlyArmor);

        public override TargetType TargetType => TargetType.Player;

        internal override bool Ethereal(int upgradeCount) => true;
        public override int CiCanCallEnergyCost(int upgradeCount) => 1;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            var blk = upgradeCount == 0 ? 10 : 13;
            ef.PlayerEffect.AddBlockStep("GhostlyArmor", blk);
        }
    }
}
