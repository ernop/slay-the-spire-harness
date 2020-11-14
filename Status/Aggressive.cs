namespace StS
{
    /// <summary>
    /// The one used by the pillbugs where they get damage when you attack them.
    /// </summary>
    public class Aggressive : Status
    {

        public override string Name => nameof(Aggressive);

        public override StatusType StatusType => StatusType.Aggressive;

        internal override void Apply(Card card, EffectSet ef, int intensity)
        {
            if (card.CardType == CardType.Attack)
            {
                ef.EnemyReceivesDamage.Add((el) =>
                {
                    if (intensity > 0)
                    {
                        if (el > 0)
                        {
                            //removal of pen nib whenever we play an attack.
                            var negativeAggressiveStatus = new StatusInstance(new Aggressive(), int.MinValue, 0);

                            //whoah, this will be applied when the attack is actually resolved.
                            //since here we're 
                            ef.EnemyStatus.Add(negativeAggressiveStatus);

                            ef.EnemyGainsBlock.Add((el) => intensity);
                        }
                    }
                    return el;
                });
            }
        }
    }
}
