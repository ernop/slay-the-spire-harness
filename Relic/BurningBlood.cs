namespace StS
{
    public class BurningBlood : Relic
    {
        public override string Name => throw new System.NotImplementedException();

        public override void EndFight(Deck d, EffectSet relicEf)
        {
            var oe = new OneEffect();
            oe.Action = (Fight f, Deck d) =>
            {
                f._Player.HealFor(6, out string healres);
                return $"Burning Blood Heal {healres}";
            };
            relicEf.FightEffect.Add(oe);
        }

        internal override Relic Copy() => new BurningBlood();
    }
}
