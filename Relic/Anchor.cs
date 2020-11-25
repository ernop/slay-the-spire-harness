﻿namespace StS
{
    public class Anchor : Relic
    {
        public override string Name => nameof(Anchor);
        private bool Used { get; set; }

        public override void CardPlayed(Card card, EffectSet ef, Entity player, Entity enemy) { }
        public override void FirstRoundStarts(Player player, Enemy enemy, EffectSet relicEf)
        {
            if (relicEf.SourceEffect.InitialBlock != 0)
            {
                throw new System.Exception("Shouldn't happen.");
            }
            relicEf.SourceEffect.InitialBlock = 10;
        }
    }
}
