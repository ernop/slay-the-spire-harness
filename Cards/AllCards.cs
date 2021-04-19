using System.Collections.Generic;

namespace StS
{

    public static class AllCards
    {

        public static Dictionary<string, Card> Cards { get; private set; } = GetAllCards();

        private static Dictionary<string, Card> GetAllCards()
        {
            var cards = new Dictionary<string, Card>();
            cards[nameof(Bash)] = new Bash();
            cards[nameof(Strike)] = new Strike();
            cards[nameof(Defend)] = new Defend();
            cards[nameof(Inflame)] = new Inflame();
            cards[nameof(LimitBreak)] = new LimitBreak();
            cards[nameof(Footwork)] = new Footwork();
            cards[nameof(IronWave)] = new IronWave();
            cards[nameof(Disarm)] = new Disarm();
            cards[nameof(Uppercut)] = new Uppercut();
            cards[nameof(SwordBoomerang)] = new SwordBoomerang();
            cards[nameof(FlameBarrier)] = new FlameBarrier();
            cards[nameof(Entrench)] = new Entrench();
            cards[nameof(BodySlam)] = new BodySlam();
            cards[nameof(Shockwave)] = new Shockwave();
            cards[nameof(Sentinel)] = new Sentinel();
            cards[nameof(Clothesline)] = new Clothesline();
            cards[nameof(SearingBlow)] = new SearingBlow();
            cards[nameof(Headbutt)] = new Headbutt();
            cards[nameof(Clash)] = new Clash();
            cards[nameof(PerfectedStrike)] = new PerfectedStrike();
            cards[nameof(TwinStrike)] = new TwinStrike();
            cards[nameof(HeavyBlade)] = new HeavyBlade();
            cards[nameof(ShrugItOff)] = new ShrugItOff();
            cards[nameof(Carnage)] = new Carnage();
            cards[nameof(Slimed)] = new Slimed();
            cards[nameof(Dazed)] = new Dazed();
            cards[nameof(PommelStrike)] = new PommelStrike();
            cards[nameof(Havok)] = new Havok();
            cards[nameof(TrueGrit)] = new TrueGrit();
            cards[nameof(FeelNoPain)] = new FeelNoPain();
            cards[nameof(Pummel)] = new Pummel();
            cards[nameof(Intimidate)] = new Intimidate();
            cards[nameof(Thunderclap)] = new Thunderclap();
            cards[nameof(Anger)] = new Anger();
            cards[nameof(WildStrike)] = new WildStrike();
            cards[nameof(Wound)] = new Wound();
            cards[nameof(Evolve)] = new Evolve();
            cards[nameof(Trip)] = new Trip();
            cards[nameof(GhostlyArmor)] = new GhostlyArmor();
            cards[nameof(RecklessCharge)] = new RecklessCharge();
            cards[nameof(Apotheosis)] = new Apotheosis();
            cards[nameof(AscendersBane)] = new AscendersBane();
            cards[nameof(Enlightenment)] = new Enlightenment();
            cards[nameof(Immolate)] = new Immolate();
            cards[nameof(Burn)] = new Burn();
            cards[nameof(Clumsy)] = new Clumsy();
            cards[nameof(SeeingRed)] = new SeeingRed();
            cards[nameof(Warcry)] = new Warcry();
            cards[nameof(Rage)] = new Rage();
            cards[nameof(BattleTrance)] = new BattleTrance();
            cards[nameof(Armaments)] = new Armaments();

            return cards;
        }
    }
}
