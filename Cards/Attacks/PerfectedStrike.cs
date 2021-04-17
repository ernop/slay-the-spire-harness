using System.Collections.Generic;
using System.Linq;

namespace StS
{

    public class PerfectedStrike : IroncladAttackCard
    {
        public override string Name => nameof(PerfectedStrike);
        public static readonly List<string> RelatedCards = new List<string>() { "PerfectedStrike", "TwinStrike", "Strike", "PommelStrike", "WildStrike", "SwiftStrike", "SneakyStrike", "ThunderStrike", "MeteorStrike", "WindmillStrike" };
        public override EnergyCostInt CiCanCallEnergyCost(int upgradeCount) => new EnergyCostInt(2);
        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            var mult = upgradeCount == 0 ? 2 : 3;
            int count = 1; //PS itself is in a nether region now - played, but not yet in discard pile.
            //don't do anything with backup cards.  Just look in draw, hand, discard.
            foreach (var list in new List<IList<CardInstance>>() { deck.GetDrawPile, deck.GetDiscardPile, deck.GetHand })
            {
                var others = list.Where(el => RelatedCards.Contains(el.Card.Name));
                count += others.Count();
            }
            var dmg = 6 + mult * count;
            ef.EnemyEffect.SetInitialDamage(dmg);
        }
    }
}
