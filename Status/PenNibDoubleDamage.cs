using System;
using System.Collections.Generic;
using System.Text;

namespace StS
{
    public class PenNibDoubleDamage : Status
    {
        public override string Name => "Double Damage from Pen Nib";

        public override StatusType StatusType => StatusType.PenNibDoubleDamage;


        internal override void Apply(Card card, IndividualEffect sourceSet, IndividualEffect targetSet, int intensity, bool statusIsTargeted)
        {
            if (card.CardType == CardType.Attack && !statusIsTargeted)
            {
                targetSet.ReceiveDamage.Add(new Progression("PenNibDD", (el) =>
                {
                    if (intensity > 0)
                    {
                        //removal of pen nib whenever we play an attack.
                        var negativePenNib = new StatusInstance(new PenNibDoubleDamage(), int.MinValue, 0);

                        //whoah, this will be applied when the attack is actually resolved.
                        //since here we're 
                        sourceSet.Status.Add(negativePenNib);

                        if (el > 0)
                        {
                            return el * 2;
                        }
                    }
                    return el;
                }));
            }
        }

        
    }
}
