using System.Collections.Generic;
using static StS.Helpers;

namespace StS
{
    public class Warcry : IroncladSkillCard
    {
        public override string Name => nameof(Warcry);

        public override TargetType TargetType => TargetType.Player;

        public override int CiCanCallEnergyCost(int upgradeCount) => 0;

 

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null)
        {
            var ct = upgradeCount == 0 ? 1 : 2;

            ef.DeckEffect.Add((Deck d, List<string> history) => {
                var drewDesc = "";
                var toDrawPileDesc = "";
                var would = d.WouldDraw(ct);
                drewDesc = SJ(input: would);
                d.ForceDrawCards(would, ef, history);

                //target is only the card we are going to throw away.
                CardInstance cardToPutOnTopOfDiscardPile;
                if (targets == null)
                {
                    cardToPutOnTopOfDiscardPile = d.GetHand[Rnd.Next(d.GetHand.Count)];
                    
                }
                else
                {
                    if (targets.Count != 1) throw new System.Exception();
                    cardToPutOnTopOfDiscardPile = targets[0];
                }
                toDrawPileDesc = cardToPutOnTopOfDiscardPile.ToString();
                d.AddToDrawPile(cardToPutOnTopOfDiscardPile);
                d.GetHand.Remove(cardToPutOnTopOfDiscardPile);

                history.Add("Added card to a random spot in draw pile");
                return $"warcry drew '{drewDesc}' and put '{toDrawPileDesc}' on top of draw pile";
            });
        }
    }
}
