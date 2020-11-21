namespace StS
{
    public class Player : Entity
    {
        public Player(CharacterType type = CharacterType.IronClad, int? hpMax = null, int? hp = null) : base("Wilson", EntityType.Player, hpMax ?? 100, hp ?? 100)
        {
            CharacterType = type;
        }

        public CharacterType CharacterType { get; }
        public int Energy { get; set; }
        public int MaxEnergy()
        {
            var value = 3;
            foreach (var relic in Relics)
            {
                if (relic.ExtraEnergy)
                {
                    value++;
                }
            }
            return value;
        }
    }
}
