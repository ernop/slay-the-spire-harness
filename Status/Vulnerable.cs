using System;

namespace StS
{
    public class Vulnerable : Status
    {
        public override StatusType StatusType => StatusType.Vulnerable;

        public override string Name => nameof(Vulnerable);

        public override int AdjustReceivedDamage(int amount, int duration, int intensity)
        {
            return (int)Math.Floor(amount * 1.5);
        }
    }
}
