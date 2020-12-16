namespace StS
{
    public abstract class Relic
    {
        public abstract string Name { get; }
        public Entity Player { get; set; }

        public virtual void CardPlayed(Card card, EffectSet ef, IEntity player, IEntity enemy) { }
        public virtual bool ExtraEnergy => false;

        public virtual void StartFight(Deck d, EffectSet ef) { }
        public virtual void EndFight(Deck d, EffectSet ef) { }

        /// <summary>
        /// when a status is added to a player to start a fight.
        /// </summary>
        public virtual void Apply(Fight f, Deck d, Player e) { }
        public virtual void Unapply(Fight f, Deck d, Player e) { }

        public virtual void StartTurn(Player player, IEntity enemy, EffectSet ef) { }
        public virtual void EndTurn(Player player, IEntity enemy, EffectSet ef) { }

        internal abstract Relic Copy();

        public override string ToString()
        {
            return $"{Name}";
        }

    }
}