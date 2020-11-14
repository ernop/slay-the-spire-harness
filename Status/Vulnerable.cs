using System;

namespace StS
{
    public class Vulnerable : Status
    {
        public override StatusType StatusType => StatusType.Vulnerable;

        public override string Name => nameof(Vulnerable);

        internal override void Apply(EffectSet set, int intensity)
        {
            if (set.EnemyReceivesDamage != null)
            {

                set.EnemyReceivesDamage.Add((el) =>
                {
                    if (el > 0)
                    {
                        return (int)Math.Floor(el * 1.5);
                    }
                    return 0;
                });
            }
        }
    }

}
