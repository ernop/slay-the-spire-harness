using System;
using System.Collections.Generic;
using System.Text;

namespace StS
{
    public class PinNibDoubleDamage : Status
    {
        public override string Name => "Double Damage from Pen Nib";

        public override StatusType StatusType => StatusType.PenNibDoubleDamage;


        internal override void Apply(EffectSet set, int intensity)
        {
            set.PlayerReceivesDamage.Add((el) => 2 * el);
        }
    }
}
