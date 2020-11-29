namespace StS
{
    public abstract class Relic
    {
        public abstract string Name { get; }
        public Entity Player { get; set; }

        public virtual void CardPlayed(Card card, EffectSet ef, IEntity player, IEntity enemy) { }
        public virtual bool ExtraEnergy => false;


        public override string ToString()
        {
            return $"{Name}";
        }

        /// <summary>
        /// called to get end of turn effectset.
        /// </summary>
        public virtual void EndTurn(Player player, IEnemy enemy, EffectSet relicEf) { }
        public virtual void StartTurn(Player player, IEnemy enemy, EffectSet relicEf) { }
        public virtual void EndFight(EffectSet relicEf) { }
        internal abstract Relic Copy();
    }
}