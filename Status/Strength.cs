namespace StS
{
    public class Strength : Status
    {
        public override string Name => nameof(Strength);

        public override StatusType StatusType => StatusType.Strength;

        internal override void Apply(EffectSet set, int intensity)
        {
            set.EnemyReceivesDamage.Add((el) =>
            {
                if (el > 0)
                {
                    return el + intensity;
                }
                return 0;
            });
            
        }
    }
}
