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
        PenNibDoubleDamage = 6,

        //start at 200 for enemy-only statuses.
        Aggressive = 200,
    }
}
