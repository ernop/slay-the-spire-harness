namespace StS
{
    public class Sentinel : SkillCard
    {
        public override string Name => nameof(Sentinel);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override TargetType TargetType => TargetType.Player;

        public override int EnergyCost(int upgradeCount) => 1;

        public override bool Ethereal(int upgradeCount) => false;

        public override bool Exhausts(int upgradeCount) => false;
        public override void OtherEffects(Action action, EffectSet ef, int upgradeCount)
        {
            if (action == Action.Exhaust)
            {
                ef.PlayerMana += upgradeCount == 0 ? 2 : 3;
            }
        }
        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount)
        {
            
                int amount;
                if (upgradeCount == 0)
                {
                    amount = 5;
                }
                else
                {
                    amount = 8;
                }
                ef.TargetEffect.InitialBlock = amount;
        }
    }
}
