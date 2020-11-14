using System;
using System.Collections.Generic;
using System.Text;

namespace StS
{
    public abstract class Relic
    {
        public abstract EffectSet CardPlayed(Card card);
    }
}
