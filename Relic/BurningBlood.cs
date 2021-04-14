using System.Collections.Generic;

namespace StS
{
    public class BurningBlood : Relic
    {
        public override string Name => nameof(BurningBlood);

        public override void EndFight(Deck d, EffectSet relicEf)
        {
            var oe = new OneEffect();

            //TODO why is this weird.  oneEffect can actually cause multiple histories.  So probably refactor the return value.

            oe.Action = (Fight f, Deck d, List<string> history) =>
            {
                f._Player.HealFor(6, out string healres);
                history.Add($"Burning Blood Heal {healres}");
            };
            relicEf.FightEffect.Add(oe);
        }

        internal override Relic Copy() => new BurningBlood();
    }
}
