using System.Collections.Generic;

namespace StS
{

    public class Deck
    {
        public List<CardInstance> Cards { get; set; }
        public List<CardInstance> DrawPile { get; set; }
        public List<CardInstance> Hand { get; set; }
        public List<CardInstance> DiscardPile { get; set; }
        public List<CardInstance> ExhaustPile { get; set; }

        //when you start a fight, copy cards into drawpile + hand;
        //some fight-actions can modify cards (receiving curse) but generally at the end of a fight you just destroy the copied cardinstances.
    }
}
