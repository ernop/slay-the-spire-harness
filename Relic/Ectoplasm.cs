namespace StS
{
    public class Ectoplasm : Relic
    {
        public override string Name => nameof(Ectoplasm);
        public override bool ExtraEnergy => true;

        internal override Relic Copy() => new Ectoplasm();
    }
}
