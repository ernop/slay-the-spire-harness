using System.Collections.Generic;

namespace StS
{
    public class RecklessCharge : IroncladAttackCard
    {
        public override string Name => nameof(RecklessCharge);
        public override bool RandomEffects => true;
        public override int CiCanCallEnergyCost(int upgradeCount) => 0;
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
            var dmg = upgradeCount == 0 ? 7 : 10;
            ef.DeckEffect.Add((Deck d, List<string> h) =>
            {
                var dazed = new CardInstance(new Dazed(), 0);
                var position = d.AddToRandomSpotInDrawPile(dazed, key);
                h.Add($"Reckless charge added dazed to draw position {position}");
            });
            ef.EnemyEffect.SetInitialDamage(dmg);
        }
    }
}
