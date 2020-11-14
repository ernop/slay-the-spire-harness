using System;
using System.Collections.Generic;
using System.Text;

namespace StS
{
    public abstract class Relic
    {
        public abstract void CardPlayed(Card card, EffectSet ef);
        public abstract void FightStarted();
        public abstract void NewTurn();
    }
}
