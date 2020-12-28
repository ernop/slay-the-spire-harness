using System.Collections.Generic;

namespace StS
{
    public class SeeingRed : IroncladSkillCard
    {
        public override string Name => nameof(SeeingRed);

        public override TargetType TargetType => TargetType.Player;

        public override int CiCanCallEnergyCost(int upgradeCount) => upgradeCount == 0 ? 1 : 0;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, int? key = null)
        {
            var oe = new OneEffect
            {
                Action =
             (Fight f, Deck d, List<string> history) =>
             {
                 f._Player.Energy += 2;
                 return "Gained 2 energy mana from SeeingRed";
             }
            };
        }
    }
}
