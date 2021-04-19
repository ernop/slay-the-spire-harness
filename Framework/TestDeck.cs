using System.Collections.Generic;

namespace StS
{
    public class TestDeck : Deck
    {
        public TestDeck(IList<CardInstance> cis, bool preserveOrder = false) : base(cis, preserveOrder)
        {
            InteractiveContext = false;
        }

        public TestDeck(IList<string> drawPile, IList<string> hand, IList<string> discardPile, IList<string> exhaustPile) : base(drawPile, hand, discardPile, exhaustPile)
        {
            InteractiveContext = true;
        }
    }
}