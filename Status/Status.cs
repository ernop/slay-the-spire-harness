﻿namespace StS
{
    public abstract class Status
    {
        public abstract string Name { get; }
        public abstract StatusType StatusType { get; }
        public abstract bool NegativeStatus { get; }
        /// <summary>
        /// if it's scalable like str/dex, or if not, like vuln.
        /// This refers to the intensity of the effect, not the duration.
        /// </summary>
        internal abstract bool Scalable { get; }

        /// <summary>
        /// When a status really gets applied.
        /// </summary>
        public virtual void Apply(Deck d, Entity e) { }

        public virtual void Unapply(Deck d, Entity e) { }

        /// <summary>
        /// Some scalable statues are permanent (str, dex) (but removed when zero), while others are impermanent like flame barrier
        /// 
        /// Scalable, impermanent: flame barrier
        /// Scalable, permanent: str, feelnopain.  in this  case the number represents the strength of the effect.
        /// Unscalable, impermanent: vuln. In this case the number represents the *duration* of the effect.
        /// Unscalable, permanent: ?
        /// 
        /// </summary>
        internal abstract bool Permanent { get; }

        internal virtual void CardWasPlayed(Card card, IndividualEffect sourceSet, IndividualEffect targetSet, int num, bool statusIsTargeted, bool playerAction) { }

        internal virtual void EndTurn(Entity parent, StatusInstance instance, EffectSet endTurnEf) { }
        internal virtual void StartTurn(Entity parent, StatusInstance instance, EffectSet endTurnEf) { }
    }
}
