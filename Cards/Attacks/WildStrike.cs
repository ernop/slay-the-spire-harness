using System.Collections.Generic;

namespace StS
{
    public class WildStrike : IroncladAttackCard
    {
        public override string Name => nameof(WildStrike);

        public override int CiCanCallEnergyCost(int upgradeCount) => 1;
        public override bool RandomEffects => true;
        public override List<FightAction> GetActions(Deck d, CardInstance ci)
        {
            var res = new List<FightAction>();
            for (var ii = 0; ii < d.GetDrawPile.Count; ii++)
            {
                var action = new FightAction(FightActionEnum.PlayCard, card: ci, key: ii);
                res.Add(action);
            }
            return res;
        }
        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null)
        {
            //I need some way to:
            // * make sure the childnode from playing this ends up in the random list (not choices)
            // * distinguish between those random entries.
            ef.DeckEffect.Add((Deck d, List<string> h) =>
            {
                var newCi = new CardInstance(new Wound(), 0);
                var position = d.AddToRandomSpotInDrawPile(newCi);
                return "Wound added to draw pile";
            });
            var dmg = upgradeCount == 0 ? 12 : 17;
            ef.EnemyEffect.SetInitialDamage(dmg);
        }
    }
}
