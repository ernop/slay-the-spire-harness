namespace StS
{

    //Why do I have this at all? so that I can identify & combine, compare statuses when necessary.
    public enum StatusType
    {
        Vulnerable = 1,
        Weakened = 2,
        Berserk = 3,
        Strength = 4,
        Dexterity = 5,
        PenNibStatus = 6,
        Weak = 7,
        FlameBarrierStatus = 8,
        PlatedArmor = 9,
        FeelNoPainStatus = 10,
        Frail = 11,
        EvolveStatus = 12,
        RageStatus = 13,

        //start at 200 for enemy-only statuses.
        Aggressive = 200,
        Feather = 201,

    }
}
