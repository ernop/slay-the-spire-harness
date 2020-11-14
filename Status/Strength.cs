namespace StS
{
    public class Strength : Status
    {
        public override string Name => nameof(Strength);

        public override StatusType StatusType => StatusType.Strength;

        internal override void Apply(Card card, EffectSet set, int intensity)
        {
            if (card.CardType == CardType.Attack)
            {
                if (set.EnemyReceivesDamage.Count == 0)
                {
                    throw new System.Exception("Why am i calculating strength without having a base damage?");
                }

                //strength always calculated immediately after initial damage.
                set.EnemyReceivesDamage.Insert(1, (el) =>
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
}
