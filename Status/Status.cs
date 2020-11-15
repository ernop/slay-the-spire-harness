using System;

namespace StS
{
    public abstract class Status
    {
        public abstract string Name { get; }
        public abstract StatusType StatusType { get; }
        public abstract bool NegativeStatus { get; }
        public abstract bool Permanent { get; }
        internal abstract void Apply(Card card, IndividualEffect sourceSet, IndividualEffect targetSet, int intensity, bool statusIsTargeted);

    }
}
