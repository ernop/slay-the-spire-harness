using System;

namespace StS
{
    public abstract class Status
    {
        public abstract string Name { get; }
        public abstract StatusType StatusType { get; }
        internal abstract void Apply(EffectSet set, int intensity);
    }
}
