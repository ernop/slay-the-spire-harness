namespace StS
{
    public class NoDraw : Status
    {
        public override string Name => nameof(NoDraw);

        public override StatusType StatusType => StatusType.NoDrawStatus;

        public override bool NegativeStatus => true;

        internal override bool Scalable => false;

        internal override bool Permanent => false;
    }
}
