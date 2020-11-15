using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StS
{
    public static class AllCards
    {
        public static Dictionary<string,Card> GetAllCards()
        {
            var cards = new Dictionary<string,Card>();
            cards["Bash"] = new Bash();
            cards["Strike"] = new Strike();
            cards["Defend"] = new Defend();
            cards["Inflame"] = new Inflame();
            cards["LimitBreak"] = new LimitBreak();
            cards["Footwork"] = new Footwork();
            cards["IronWave"] = new IronWave();
            cards["Disarm"] = new Disarm();
            cards["Uppercut"] = new Uppercut();
            cards["SwordBoomerang"] = new SwordBoomerang();
            cards["FlameBarrier"] = new FlameBarrier();
            cards["Entrench"] = new Entrench();
            return cards;
        }
    }
}
