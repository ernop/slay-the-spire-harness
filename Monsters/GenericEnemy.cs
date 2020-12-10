namespace StS
{
    public class GenericEnemy : Enemy, IEnemy
    {
        public int Amount { get; set; }
        public int Count { get; set; }


        public GenericEnemy(int amount = 3, int count = 3, int? hpMax = null, int? hp = null) : base(nameof(GenericEnemy), hpMax ?? 50, hp ?? 50)
        {
            Amount = amount;
            Count = count;
        }

        public override IEnemy Copy()
        {
            var res = new GenericEnemy(Amount, Count, HP, HPMax);
            CopyEntity(res);
            return res;
        }

        public override EnemyAction GetAction()
        {
            return new EnemyAction(null, new EnemyAttack(Amount, Count), null);
        }
    }
}
