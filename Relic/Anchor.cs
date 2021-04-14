﻿namespace StS
{
    public class Anchor : Relic
    {
        public override string Name => nameof(Anchor);
        internal int RoundNumber { get; set; }

        public override void StartTurn(Player player, IEntity enemy, EffectSet relicEf)
        {
            RoundNumber++;

            if (RoundNumber == 1)
            {
                relicEf.PlayerEffect.AddBlockStep("Anchor", 10);
            }
        }

        internal override Relic Copy()
        {
            return new Anchor
            {
                RoundNumber = RoundNumber
            };
        }
    }
}
