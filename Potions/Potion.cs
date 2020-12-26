﻿namespace StS
{
    public abstract class Potion
    {
        public abstract string Name { get; }
        public abstract void Apply(Fight f, Player player, IEnemy enemy, EffectSet ef);
        public abstract bool SelfTarget { get; }
        internal abstract Potion Copy();
        /// <summary>
        /// Does this potion have random effects?
        /// </summary>
        public virtual bool Random => false;
        public override string ToString()
        {
            return Name;
        }
    }
}
