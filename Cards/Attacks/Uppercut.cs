namespace StS
{
    public class Uppercut : AttackCard
    {
        public override string Name => nameof(Uppercut);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override bool Ethereal(int upgradeCount) => false;

        public override bool Exhausts(int upgradeCount) => false;

        internal override EffectSet Apply(Entity source, Entity target, int upgradeCount)
        {
            var damage = 13;
            int weak;
            int vuln;
            if (upgradeCount == 0)
            {
                weak = 1;
                vuln = 1;
            }
            else
            {
                weak = 2;
                vuln = 2;
            }
            var ef = new EffectSet();
            ef.TargetEffect.ReceiveDamage.Add(new Progression(Name, (_) => 13));
            ef.TargetEffect.Status.Add(new StatusInstance(new Vulnerable(), vuln, int.MaxValue));
            ef.TargetEffect.Status.Add(new StatusInstance(new Weak(), weak, int.MaxValue));
            
            return ef;
        }
    }
}
