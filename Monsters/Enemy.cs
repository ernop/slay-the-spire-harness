namespace StS
{

    public abstract class Enemy : Entity, IEnemy
    {
        public Enemy(string name = null, int? hp = null, int? hpMax = null) : base(name ?? "Nameless", EntityType.Enemy, hp, hpMax) { }

        public abstract IEnemy Copy();

        public abstract FightAction GetAction(int turn);
    }
}
