﻿namespace StS
{
    public class Ectoplasm : Relic
    {
        public override string Name => nameof(Ectoplasm);
        public override bool ExtraEnergy => true;

        internal override Relic Copy() => new Ectoplasm();
    }

    public class NeowsLament : Relic
    {
        public override string Name => nameof(NeowsLament);
        public int Charges { get; set; } = 3;

        public override void StartFight(Deck d, EffectSet ef)
        {
            if (Charges > 0)
            {
                Charges--;
                ef.FightEffects.Add(new OneEffect()
                {
                    Action = (Fight f, Deck d) =>
                    {
                        f.SetEnemyHp(1);
                    }
                });
            }
        }

        internal override Relic Copy()
        {
            return new NeowsLament
            {
                Charges = Charges
            };
        }
    }
}
