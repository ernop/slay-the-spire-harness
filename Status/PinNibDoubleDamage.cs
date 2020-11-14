using System;
using System.Collections.Generic;
using System.Text;

namespace StS
{
    public class PinNibDoubleDamage : Status
    {
        public override string Name => "Double Damage from Pen Nib";

        public override StatusType StatusType => StatusType.PenNibDoubleDamage;


        internal override void Apply(Card card, EffectSet ef, int intensity)
        {
            if (card.CardType == CardType.Attack)
            {
                ef.EnemyReceivesDamage.Add((el) =>
                {
                    if (intensity > 0)
                    {
                        //removal of pen nib whenever we play an attack.
                        var negativePinNib = new StatusInstance(new PinNibDoubleDamage(), -1, int.MaxValue);

                        //whoah, this will be applied when the attack is actually resolved.
                        //since here we're 
                        ef.PlayerStatus.Add(negativePinNib);

                        if (el > 0)
                        {
                            return el * 2;
                        }
                    }
                    return el;
                });
            }
        }

        
    }
}
