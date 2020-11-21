using System.Collections.Generic;

namespace StS
{

    public class Deck
    {
        public Deck(List<CardInstance> cis)
        {
            Cards = cis;
        }

        /// <summary>
        /// The actual cards in the deck; the rest are copies.
        /// </summary>
        public List<CardInstance> Cards { get; set; }
        public List<CardInstance> DrawPile { get; set; }
        
        /// <summary>
        /// These are copies for just this turn;
        /// </summary>
        public List<CardInstance> Hand { get; set; }
        public List<CardInstance> DiscardPile { get; set; }
        public List<CardInstance> ExhaustPile { get; set; }

        //when you start a fight, copy cards into drawpile + hand;
        //some fight-actions can modify cards (receiving curse) but generally at the end of a fight you just destroy the copied cardinstances.
    }
}
