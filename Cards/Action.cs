namespace StS
{
    /// <summary>
    /// All things that can happen to a card - being removed from the deck, drawn, exhausted, discarded.
    /// When something happens to a card, call card.Apply(action) and generate an effectset.
    /// 
    /// Q: Should effectset be really broad and cover being normally played, too?
    /// </summary>
    public enum Action
    {
        Play = 1,
        Exhaust = 2,
        Drawn = 3, //for void
        Discarded = 4,
        AttemptPlay = 5, //for Clash
    }
}
