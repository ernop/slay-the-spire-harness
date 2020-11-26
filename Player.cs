using System;
using System.Collections.Generic;

namespace StS
{
    public class Player : Entity
    {
        public Player(CharacterType type = CharacterType.IronClad, int? hpMax = null, int? hp = null, List<Relic> relics = null) : base("Wilson", EntityType.Player, hpMax ?? 100, hp ?? 100)
        {
            CharacterType = type;
            if (relics != null)
            {
                foreach (var relic in relics)
                {
                    relic.Player = this;
                    Relics.Add(relic);
                }
            }
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

        internal int GetDrawAmount()
        {
            ///todo this should be affected by statuses.
            return 5;
        }

        internal void HealFor(int amount)
        {
            HP = Math.Min(HPMax, HP + amount);
        }
    }
}
