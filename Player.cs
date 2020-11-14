namespace StS
{
    public class Player : Entity
    {
        public Player(GameContext gameContext, int hpMax, int hp) : base("Wilson", gameContext, EntityType.Player, hpMax, hp) { }

    }
}
