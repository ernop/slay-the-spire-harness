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
        /// 
        /// Should I add player to this?  So that the deck will know where to send exhaust events?
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

        internal bool DiscardPileContains(CardInstance headbuttTarget)
        {
            return DiscardPile.Contains(headbuttTarget);
        }

        internal void MoveFromDiscardToDraw(CardInstance headbuttTarget)
        {
            DiscardPile.Remove(headbuttTarget);
            DrawPile.Add(headbuttTarget);
        }

        internal CardInstance GetRandomCardFromHand()
        {
            if (Hand.Count == 0)
            {
                return null;
            }
            var res = Hand[Helpers.Rnd.Next(Hand.Count)];
            return res;
        }

        public List<CardInstance> BackupCards { get; private set; }

        /// <summary>
        /// TargetCards are forced choice even if the choice ought to be random.
        /// </summary>
        internal List<CardInstance> DrawToHand(List<CardInstance> targetCards, int count, bool reshuffle, EffectSet ef)
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
                            Reshuffle(ef);
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

            return res;
        }

        internal List<CardInstance> Draw(List<CardInstance> targetCards, int count, bool reshuffle, EffectSet ef)
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
                            Reshuffle(ef);
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
        private List<CardInstance> DrawPile { get; set; }
        private List<CardInstance> Hand { get; set; } = new List<CardInstance>();
        private List<CardInstance> DiscardPile { get; set; } = new List<CardInstance>();
        private List<CardInstance> ExhaustPile { get; set; } = new List<CardInstance>();

        /// <summary>
        /// should these all just return by value?  or do I need to modify them in tests?
        /// </summary>
        public IList<CardInstance> GetDrawPile => DrawPile;
        public IList<CardInstance> GetHand => Hand;
        public IList<CardInstance> GetDiscardPile => DiscardPile;
        public IList<CardInstance> GetExhaustPile => ExhaustPile;

        public event NotifyOfExhaustion ExhaustCard;

        public delegate void NotifyOfExhaustion(EffectSet ef);

        public event NotifyDeckShuffle DeckShuffle;
        public delegate void NotifyDeckShuffle(EffectSet ef);

        internal Deck(List<CardInstance> hand, List<CardInstance> draw, List<CardInstance> discard, List<CardInstance> ex)
        {
            Hand = hand;
            DrawPile = draw;
            DiscardPile = discard;
            ExhaustPile = ex;
        }

        public void ShuffleDrawPile()
        {
            DrawPile = DrawPile.OrderBy(el => Rnd.Next()).ToList();
        }

        //when you start a fight, copy cards into drawpile + hand;
        //some fight-actions can modify cards (receiving curse) but generally at the end of a fight you just destroy the copied cardinstances.

        public void TurnEnds(EffectSet ef)
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
                    Exhaust(ci, ef);
                }
                else
                {
                    PutInDiscardPile(ci);
                }
            }
        }

        public void Exhaust(CardInstance ci, EffectSet ef)
        {
            ci.OtherAction(Action.Exhaust, ef);

            //this has to somehow hook into relics that care about exhaust, and also player / monster statuses which care too.
            ExhaustCard?.Invoke(ef);
            ExhaustPile.Add(ci);
        }

        public void ExhaustFromHand(CardInstance ci, EffectSet ef)
        {
            if (ci == null)
            {
                return;
            }
            Hand.Remove(ci);
            Exhaust(ci, ef);
        }

        /// <summary>
        /// Doesn't trigger "Discard" events.
        /// </summary>
        public void PutInDiscardPile(CardInstance ci)
        {
            DiscardPile.Add(ci);
        }

        public void NextTurnStarts(int drawCount, EffectSet ef)
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
                    Reshuffle(ef);
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

        internal void AfterPlayingCard(CardInstance ci, EffectSet ef)
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
                    Exhaust(ci, ef);
                }
                else
                {
                    PutIntoDiscardAfterApplyingEffectSet = ci;
                }
            }
        }

        internal void FinishCardPlay()
        {
            if (PutIntoDiscardAfterApplyingEffectSet != null)
            {
                DiscardPile.Add(PutIntoDiscardAfterApplyingEffectSet);
                PutIntoDiscardAfterApplyingEffectSet = null;
            }
        }

        private CardInstance PutIntoDiscardAfterApplyingEffectSet { get; set; }

        public void Reshuffle(EffectSet ef)
        {
            DeckShuffle?.Invoke(ef);
            DrawPile.AddRange(DiscardPile);
            DiscardPile = new List<CardInstance>();
            ShuffleDrawPile();
        }

        public void FightEnded()
        {

        }

        internal Deck Copy()
        {
            var h = Hand.Select(el => el.Copy()).ToList();
            var dr = DrawPile.Select(el => el.Copy()).ToList();
            var dis = DiscardPile.Select(el => el.Copy()).ToList();
            var ex = ExhaustPile.Select(el => el.Copy()).ToList();
            var d = new Deck(h, dr, dis, ex);
            return d;
        }
    }
}
