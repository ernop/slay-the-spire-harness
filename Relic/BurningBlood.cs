namespace StS
{
    public class BurningBlood : Relic
    {
        public override string Name => throw new System.NotImplementedException();

        public override void EndFight(Deck d, EffectSet relicEf)
        {
            relicEf.PlayerEffect.Add((Player p) =>
            {
                p.HealFor(6, out string healRes);
                return $"{Player} healRes";
            });
        }

        internal override Relic Copy() => new BurningBlood();
    }
}
