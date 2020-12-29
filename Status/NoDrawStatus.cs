namespace StS
{
    public class NoDrawStatus : Status
    {
        public override string Name => nameof(NoDrawStatus);

        public override StatusType StatusType => StatusType.NoDrawStatus;

        public override bool NegativeStatus => true;

        internal override bool Scalable => false;

        internal override bool Permanent => false;
    }
}
