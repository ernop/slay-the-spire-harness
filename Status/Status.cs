﻿using System;

namespace StS
{
    public abstract class Status
    {
        public abstract string Name { get; }
        public abstract StatusType StatusType { get; }
        internal abstract void Apply(Card card, EffectSet set, int intensity);
    }
}
