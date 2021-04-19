using System.Collections.Generic;

namespace StS
{
    public class WildStrike : IroncladAttackCard
    {
        public override string Name => nameof(WildStrike);

        public override EnergyCostInt CiCanCallEnergyCost(int upgradeCount) => new EnergyCostInt(1);
        public override bool RandomEffects => true;
        public override List<int> GetKeys(Deck d, CardInstance ci)
        {
            var res = new List<int>();
            for (var ii = 0; ii < d.GetDrawPile.Count + 1; ii++)
            {
                res.Add(ii);
            }
            return res;
        }
        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            //I need some way to:
            // * make sure the childnode from playing this ends up in the random list (not choices)
            // * distinguish between those random entries.
            ef.DeckEffect.Add((Deck d, List<string> h) =>
            {
                var newCi = new CardInstance(new Wound(), 0);
                var position = d.AddToRandomSpotInDrawPile(newCi, key);
                h.Add($"Wound added to draw pile position:{position}");
            });
            var dmg = upgradeCount == 0 ? 12 : 17;
            ef.EnemyEffect.SetInitialDamage(dmg);
            ef.HadRandomness = true;
            ef.Key = key ?? 0;
        }
    }
}
