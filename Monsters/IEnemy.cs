namespace StS
{
    public interface IEnemy : IEntity
    {
        public abstract FightAction GetAction(int turn);
        public abstract IEnemy Copy();
    }
}
