﻿namespace StS
{
    public class Ectoplasm : Relic
    {
        public override string Name => nameof(Ectoplasm);
        public override bool ExtraEnergy => true;
    }

    public class BustedCrown : Relic
    {
        public override string Name => nameof(BustedCrown);

        public override void CardPlayed(Card card, EffectSet ef, Entity player, Entity enemy) { }

        public override bool ExtraEnergy => true;
    }
}
