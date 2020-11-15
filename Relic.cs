using System;
using System.Collections.Generic;
using System.Text;

namespace StS
{
    public abstract class Relic
    {
        public abstract string Name { get; }
        public abstract void CardPlayed(Card card, EffectSet ef, Entity player, Entity enemy);
        public Entity Player { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
