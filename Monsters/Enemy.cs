namespace StS
{

    public abstract class Enemy : Entity, IEnemy
    {
        public Enemy(string? name = null, int? hpMax = null, int? hp = null) : base(name ?? "Nameless", EntityType.Enemy, hpMax ?? 50, hp ?? 50) { }

        public abstract IEnemy Copy();

        public abstract EnemyAction GetAction();
    }
}
