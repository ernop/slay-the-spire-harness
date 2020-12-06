using System.Collections.Generic;

namespace StS
{
    public static class AllCards
    {

        public static Dictionary<string, Card> Cards { get; private set; } = GetAllCards();

        private static Dictionary<string, Card> GetAllCards()
        {
            var cards = new Dictionary<string, Card>();
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
            cards["BodySlam"] = new BodySlam();
            cards["Shockwave"] = new Shockwave();
            cards["Sentinel"] = new Sentinel();
            cards["Clothesline"] = new Clothesline();
            cards["SearingBlow"] = new SearingBlow();
            cards["Headbutt"] = new Headbutt();
            cards["Clash"] = new Clash();
            cards["PerfectedStrike"] = new PerfectedStrike();
            cards["TwinStrike"] = new TwinStrike();
            cards["HeavyBlade"] = new HeavyBlade();
            cards["ShrugItOff"] = new ShrugItOff();
            cards["Carnage"] = new Carnage();
            cards["Slimed"] = new Slimed();
            cards["Dazed"] = new Dazed();
            cards["PommelStrike"] = new PommelStrike();
            cards["Havok"] = new Havok();
            cards["TrueGrit"] = new TrueGrit();
            cards["FeelNoPain"] = new FeelNoPain();
            cards["Pummel"] = new Pummel();
            cards["Intimidate"] = new Intimidate();

            return cards;
        }
    }
}
