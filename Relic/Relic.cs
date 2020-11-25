namespace StS
{
    public abstract class Relic
    {
        public abstract string Name { get; }
        public abstract void CardPlayed(Card card, EffectSet ef, Entity player, Entity enemy);
        public virtual bool ExtraEnergy => false;
        public Entity Player { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }

        /// <summary>
        /// called to get end of turn effectset.
        /// </summary>
        public virtual void EndTurn(Player player, Enemy enemy, EffectSet relicEf)
        {
        }

        public virtual void FirstRoundStarts(Player player, Enemy enemy, EffectSet relicEf)
        {
        }
    }
}
