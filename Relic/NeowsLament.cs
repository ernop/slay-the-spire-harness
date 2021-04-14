using System.Collections.Generic;

namespace StS
{
    public class NeowsLament : Relic
    {
        public override string Name => nameof(NeowsLament);
        public int Charges { get; set; } = 3;

        public override void StartFight(Deck d, EffectSet ef)
        {
            if (Charges > 0)
            {
                Charges--;
                ef.FightEffect.Add(new OneEffect()
                {
                    Action = (Fight f, Deck d, List<string> history) =>
                    {
                        f.SetEnemyHp(1);
                        history.Add("Set enemy HP to 1");
                    }
                });
            }
        }
        public override string ToString()
        {
            return $"{nameof(NeowsLament)}:{Charges}";
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
