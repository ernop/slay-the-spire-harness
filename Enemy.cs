using System;

namespace StS
{
    public class Enemy : Entity
    {
        public Enemy(string? name = null, int? hpMax = null, int? hp = null) : base(name ?? "Nameless", EntityType.Enemy, hpMax ?? 50, hp ?? 50)
        {
        }

        public virtual EnemyAction GetAction()
        {
            throw new NotImplementedException();
        }
    }
}
