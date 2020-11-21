namespace StS
{
    public class Player : Entity
    {
        public Player(int? hpMax = null, int? hp = null) : base("Wilson", EntityType.Player, hpMax ?? 100 , hp ?? 100) { }

    }
}
