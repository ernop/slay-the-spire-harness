using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        public Deck([NotNull] List<CardInstance> cis, bool preserveOrder = false)
        {
            BackupCards = cis?.Select(el => CopyCI(el)).ToList();
            var newList = new List<CardInstance>();

            //we want the original external cis list to still work, but we also want to use the original external cards in here.
            newList.AddRange(cis);

            DrawPile = newList;
            if (!preserveOrder)
            {
                ShuffleDrawPile();
            }
        }




        public List<CardInstance> BackupCards { get; private set; }

        /// <summary>
        /// TargetCards are forced choice even if the choice ought to be random.
        /// </summary>
        internal void DrawToHand(Card card, List<CardInstance> targetCards, int count, bool reshuffle)
        {
            var res = new List<CardInstance>() { };


            if (targetCards == null)
            {

                while (res.Count < count)
                {
                    if (DrawPile.Count == 0)
                    {
                        if (reshuffle)
                        {
                            Reshuffle();
                            if (DrawPile.Count == 0)
                            {
                                break;
                            }
                        }
                        else
                        {
                            //can't pull more and can't reshuffle (pull from draw pile only, for example.)
                            break;
                        }
                    }

                    var pulledCard = DrawPile[DrawPile.Count - 1];
                    DrawPile.Remove(pulledCard);
                    res.Add(pulledCard);
                }
            }
            else
            {
                res = targetCards;
            }

            foreach (var effectedCard in res)
            {
                TryAddToHand(effectedCard);
            }
        }

        internal List<CardInstance> Draw(Card card, List<CardInstance> targetCards, int count, bool reshuffle)
        {
            var res = new List<CardInstance>() { };
            if (targetCards == null)
            {

                while (res.Count < count)
                {
                    if (DrawPile.Count == 0)
                    {
                        if (reshuffle)
                        {
                            Reshuffle();
                            if (DrawPile.Count == 0)
                            {
                                break;
                            }
                        }
                        else
                        {
                            //can't pull more and can't reshuffle (pull from draw pile only, for example.)
                            break;
                        }
                    }

                    var pulledCard = DrawPile[DrawPile.Count - 1];
                    DrawPile.Remove(pulledCard);
                    res.Add(pulledCard);
                }
            }
            else
            {
                res = targetCards;
            }

            return res;
        }

        /// <summary>
        /// add to hand if hand isn't too bigyet.
        /// </summary>
        private void TryAddToHand(CardInstance ci)
        {
            if (Hand.Count < 10)
            {
                Hand.Add(ci);
            }
            else
            {
                DiscardPile.Add(ci);
            }
        }


        /// <summary>
        /// The actual cards in the deck; the rest are copies.
        /// </summary>
        public List<CardInstance> DrawPile { get; private set; }

        /// <summary>
        /// These are copies for just this turn;
        /// </summary>
        public List<CardInstance> Hand { get; private set; } = new List<CardInstance>();
        public List<CardInstance> DiscardPile { get; private set; } = new List<CardInstance>();
        public List<CardInstance> ExhaustPile { get; private set; } = new List<CardInstance>();

        public void ShuffleDrawPile()
        {
            var r = new Random();
            DrawPile = DrawPile.OrderBy(el => r.Next()).ToList();
        }



        //when you start a fight, copy cards into drawpile + hand;
        //some fight-actions can modify cards (receiving curse) but generally at the end of a fight you just destroy the copied cardinstances.

        public void TurnEnds()
        {
            //clear 
            //shuffle discard back into draw.
            while (Hand.Count > 0)
            {

                var ci = Hand[0];
                ci.LeavingHand();
                Hand.Remove(ci);
                if (ci.Ethereal())
                {
                    ExhaustPile.Add(ci);
                }
                else
                {
                    DiscardPile.Add(ci);
                }

            }
        }

        public void NextTurnStarts(int drawCount)
        {
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
                    Reshuffle();
                    continue;
                }
                toDraw--;
            }
        }

        internal void BeforePlayingCard(CardInstance ci)
        {
            ///Todo this will be a problem with playing cards that have just been created.
            if (!Hand.Contains(ci))
            {
                throw new Exception("playing card you don't have");
            }
            Hand.Remove(ci);

        }

        internal void AfterPlayingCard(CardInstance ci)
        {
            if (ci.Card.CardType == CardType.Power)
            {
                //card just disappears.
            }
            else
            {
                ci.LeavingHand();
                if (ci.Exhausts())
                {
                    ExhaustPile.Add(ci);
                }
                else
                {
                    DiscardPile.Add(ci);
                }
            }
        }

        public void Reshuffle()
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
