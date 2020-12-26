using System.Collections.Generic;
using System.Linq;

namespace StS
{

    public class PerfectedStrike : IroncladAttackCard
    {
        public override string Name => nameof(PerfectedStrike);
        public static readonly List<string> RelatedCards = new List<string>() { "PerfectedStrike", "TwinStrike", "Strike", "PommelStrike", "WildStrike", "SwiftStrike", "SneakyStrike", "ThunderStrike", "MeteorStrike", "WindmillStrike" };
        public override int CiCanCallEnergyCost(int upgradeCount) => 2;
        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null)
        {
            var mult = upgradeCount == 0 ? 2 : 3;

            var others = deck.BackupCards.Where(el => RelatedCards.Contains(el.Card.Name)).Count();
            var dmg = 6 + mult * others;
            ef.EnemyEffect.SetInitialDamage(dmg);
        }
    }
}
