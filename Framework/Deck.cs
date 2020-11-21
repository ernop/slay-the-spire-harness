using System;
using System.Collections.Generic;
using System.Linq;

using static StS.Helpers;

namespace StS
{


    public class Deck
    {
        /// <summary>
        /// initial deck state: all cards in "DrawPile"
        /// Note that we put them in order based on order submitted - no randomization.
        /// This will allow for much easier tests, but means I have to remember to randomize them when doing real fights.
        /// 
        /// Note that per-hand cards are the same as per-fight; but at the end of the fight we should restore the backup cards or something?
        /// makes sense to store the deck as the canonical list.
        /// </summary>
        public Deck(List<CardInstance> cis, bool preserveOrder = false)
        {
            BackupCards = cis.Select(el => CopyCI(el)).ToList();
            var newList = new List<CardInstance>();

            //we want the original external cis list to still work, but we also want to use the original external cards in here.
            newList.AddRange(cis);
            DrawPile = newList;
            if (!preserveOrder)
            {
                ShuffleDrawPile();
            }
        }




        public List<CardInstance> BackupCards { get; set; }

        /// <summary>
        /// The actual cards in the deck; the rest are copies.
        /// </summary>
        public List<CardInstance> DrawPile { get; set; }

        /// <summary>
        /// These are copies for just this turn;
        /// </summary>
        public List<CardInstance> Hand { get; set; } = new List<CardInstance>();
        public List<CardInstance> DiscardPile { get; set; } = new List<CardInstance>();
        public List<CardInstance> ExhaustPile { get; set; } = new List<CardInstance>();

        public void ShuffleDrawPile()
        {
            var r = new Random();
            DrawPile = DrawPile.OrderBy(el => r.Next()).ToList();
        }



        //when you start a fight, copy cards into drawpile + hand;
        //some fight-actions can modify cards (receiving curse) but generally at the end of a fight you just destroy the copied cardinstances.

        public void NextTurn(int drawCount)
        {
            //clear 
            //shuffle discard back into draw.
            foreach (var ci in Hand)
            {
                ci.EnteringDiscardPile();
            }
            DiscardPile.AddRange(Hand);
            Hand = new List<CardInstance>();

            int toDraw = drawCount;
            while (toDraw > 0)
            {
                if (DrawPile.Count > 0)
                {
                    var el = DrawPile.Last();
                    DrawPile.Remove(el);
                    Hand.Add(el);
                }
                else
                {
                    if (DiscardPile.Count == 0)
                    {
                        //can't do a full draw.
                        break;
                    }
                    ReshuffleDiscards();
                    continue;
                }
                toDraw--;
            }
        }

        internal void PlayingCard(CardInstance cardInstance)
        {
            if (!Hand.Contains(cardInstance))
            {
                throw new Exception("playing card you don't have");
            }
            Hand.Remove(cardInstance);
            Console.WriteLine(Hand.Contains(cardInstance));
            if (cardInstance.Card.CardType == CardType.Power)
            {
                //card just disappears.
            }
            else
            {
                cardInstance.EnteringDiscardPile();
                DiscardPile.Add(cardInstance);
            }
        }

        private void ReshuffleDiscards()
        {
            DrawPile.AddRange(DiscardPile);
            DiscardPile = new List<CardInstance>();
            ShuffleDrawPile();
        }

        public void FightEnded()
        {

        }
    }
}
