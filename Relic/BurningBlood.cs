namespace StS
{
    public class BurningBlood : Relic
    {
        public override string Name => throw new System.NotImplementedException();

        public override void EndFight(EffectSet relicEf)
        {
            relicEf.PlayerEffect.Add((Player p) =>
            {
                p.HealFor(6);
            });
        }

    }
}
