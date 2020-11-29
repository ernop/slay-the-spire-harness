namespace StS
{
    public interface IEnemy : IEntity
    {
        public abstract EnemyAction GetAction();
        public abstract IEnemy Copy();
    }
}
