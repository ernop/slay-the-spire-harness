﻿using System.Linq;

namespace StS
{
    public class PenNibStatus : Status
    {
        public override string Name => "Double Damage from Pen Nib";

        public override StatusType StatusType => StatusType.PenNibStatus;

        public override bool NegativeStatus => false;

        internal override bool Permanent => false;
        internal override bool Scalable => false;

        internal override void CardWasPlayed(Card card, IndividualEffect playerSet, IndividualEffect enemySet, int intensity, bool statusIsTargeted, bool playerAction)
        {
            if (card.CardType == CardType.Attack && !statusIsTargeted && enemySet.InitialDamage != null)
            {
                enemySet.DamageAdjustments.Add(new AttackProgression("PenNibDD", (el) =>
                {
                    if (intensity > 0)
                    {
                        //removal of pen nib whenever we play an attack.
                        var negativePenNib = new StatusInstance(new PenNibStatus(), -1);

                        //whoah, this will be applied when the attack is actually resolved.
                        //since here we're 
                        playerSet.Status.Add(negativePenNib);

                        return el.Select(qq => qq > 0 ? qq * 2 : 0).ToList();
                    }
                    return el;
                }));
            }
        }
    }
}
