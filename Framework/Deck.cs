using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;

using static StS.Helpers;

namespace StS
{
    public class Deck
    {
        /// <summary>
        /// is there a player here so we can do interactive stuff? or test/ai/sim so we just pick randomly (or later, take a specified "choice").
        /// </summary>
        public bool InteractiveContext { get; set; }

        /// <summary>
        /// if you want a certain hand to be drawn, create the deck with those cards in the drawpile, and set this.
        /// </summary>
        private bool _PreserveOrder { get; set; }

        private IList<CardInstance> DrawPile { get; set; }
        private IList<CardInstance> Hand { get; set; } = new List<CardInstance>();
        private IList<CardInstance> DiscardPile { get; set; } = new List<CardInstance>();
        private IList<CardInstance> ExhaustPile { get; set; } = new List<CardInstance>();

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
        public delegate void NotifyDeckShuffle(EffectSet ef, List<string> history);

        public event NotifyDrawCard DrawCard;
        public delegate void NotifyDrawCard(CardInstance ci, EffectSet ef);
        public Deck(IList<string> drawPile, IList<string> hand, IList<string> discardPile, IList<string> exhaustPile)
        {
            InteractiveContext = true;
            Init(GetCis(drawPile.ToArray()), GetCis(hand.ToArray()), GetCis(discardPile.ToArray()), GetCis(exhaustPile.ToArray()));
        }

        public Deck(IList<CardInstance> drawPile, IList<CardInstance> hand, IList<CardInstance> discardPile, IList<CardInstance> exhaustPile)
        {
            InteractiveContext = true;
            Init(drawPile, hand, discardPile, exhaustPile);
        }

        private void Init(IList<CardInstance> drawPile, IList<CardInstance> hand, IList<CardInstance> discardPile, IList<CardInstance> exhaustPile)
        {
            _PreserveOrder = false;
            var cis = new List<CardInstance>();
            cis.AddRange(drawPile);
            cis.AddRange(hand);
            cis.AddRange(discardPile);
            cis.AddRange(exhaustPile);

            BackupCards = cis?.Select(el => el.Copy()).ToList();
            DrawPile = drawPile;
            Hand = hand;
            DiscardPile = discardPile;
            ExhaustPile = exhaustPile;
        }
        public Deck([NotNull] IList<CardInstance> cis, bool preserveOrder = false)
        {
            InteractiveContext = true;
            _PreserveOrder = preserveOrder;
            BackupCards = cis?.Select(el => el.Copy()).ToList();
            var newList = new List<CardInstance>();

            //we want the original external cis list to still work, but we also want to use the original external cards in here.
            newList.AddRange(cis);
            DrawPile = newList;
        }

        public List<CardInstance> FindSetOfCards(IList<CardInstance> source, List<string> target)
        {
            var res = new List<CardInstance>();
            foreach (var s in target)
            {
                var exi = FindIdenticalCardInSource(source, GetCi(s), res);
                res.Add(exi);
            }
            return res;
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

        internal CardInstance GetRandomCardFromHand(Func<CardInstance, bool> filter = null)
        {
            var candidates = Hand;
            if (filter != null)
            {
                candidates = Hand.Where(el => filter.Invoke(el)).ToList();
            }

            if (candidates.Count == 0)
            {
                return null;
            }

            var res = candidates[Helpers.Rnd.Next(candidates.Count)];
            return res;
        }

        /// <summary>
        /// call this when you want to prompt the player to make a choice, or make a random choice if you're in ai mode.
        /// </summary>
        internal CardInstance ChooseCardFromHand(Func<CardInstance, bool> filter, string prompt)
        {
            if (!InteractiveContext)
            {
                return GetRandomCardFromHand(filter);
            }
            var candidates = Hand;
            if (filter != null)
            {
                candidates = Hand.Where(el => filter.Invoke(el)).ToList();
            }

            if (candidates.Count == 0)
            {
                return null;
            }
            var res = PromptPlayerToChooseFromCardInstances(candidates, prompt);

            return res;
        }

        private CardInstance PromptPlayerToChooseFromCardInstances(IList<CardInstance> cis, string prompt)
        {
            if (cis.Count == 0)
            {
                throw new Exception();
            }
            var ii = 0;
            Console.WriteLine($"Pick a card for: {prompt}");
            while (true)
            {
                while (ii < cis.Count)
                {
                    Console.WriteLine($"{ii + 1} - {cis[ii]}");
                    ii++;
                }
                var input = Console.ReadLine();
                var ok = Int32.TryParse(input, out int res);
                if (ok && res < cis.Count && res >= 0)
                {
                    return cis[res - 1];
                }
            }

        }

        public IList<CardInstance> BackupCards { get; private set; }

        /// <summary>
        /// TargetCards are forced choice even if the choice ought to be random.
        /// Note: this needs to take into account nodraw effects.
        /// </summary>
        internal List<CardInstance> DrawToHand(IList<CardInstance> targetCards, int count, bool reshuffle, Player player, EffectSet ef, List<string> history)
        {
            var res = new List<CardInstance>() { };

            if (HasNoDrawStatus(player))
            {
                history.Add("No drawing due to nodraw status.");
                return res;
            }
            if (targetCards == null)
            {
                while (res.Count < count)
                {
                    if (DrawPile.Count == 0)
                    {
                        if (reshuffle)
                        {
                            Reshuffle(ef, history);
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
                res = targetCards.ToList();
            }

            foreach (var effectedCard in res)
            {
                TryAddToHand(effectedCard, ef);
            }

            return res;
        }

        internal List<CardInstance> Draw(Player player, List<CardInstance> targetCards, int count, bool reshuffle, EffectSet ef, List<string> history)
        {
            var res = new List<CardInstance>() { };
            if (HasNoDrawStatus(player))
            {
                history.Add("No drawing due to nodraw status.");
                return res;
            }
            if (targetCards == null)
            {
                while (res.Count < count)
                {
                    if (DrawPile.Count == 0)
                    {
                        if (reshuffle)
                        {
                            Reshuffle(ef, history);
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

        internal void StartFight()
        {
            //TODO how to be sure we cover the entire deck? what configuration is it in now?
            //Idea is the deck has continuity over time and is only created at the very start.
            foreach (var ci in DrawPile)
            {
                ci.StartFight();
            }
        }

        /// <summary>
        /// add to hand if hand isn't too bigyet.
        /// </summary>
        private void TryAddToHand(CardInstance ci, EffectSet ef)
        {

            if (Hand.Count < 10)
            {
                Hand.Add(ci);
                DrawCard?.Invoke(ci, ef);
            }
            else
            {
                DiscardPile.Add(ci);
            }
        }

        /// <summary>
        /// The actual cards in the deck; the rest are copies.
        /// </summary>

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

        public void TurnEnds(EffectSet ef, List<string> history)
        {
            //clear 
            //shuffle discard back into draw.

            while (Hand.Count > 0)
            {
                var ci = Hand[0];
                ci.LeavingHand(ef);
                Hand.Remove(ci);
                if (ci.Ethereal())
                {
                    Exhaust(ci, ef);
                    history.Add($"Etheral {ci} exhausted");
                }
                else
                {
                    AddToDiscardPile(ci);
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

        public void ExhaustFromHand(CardInstance ci, EffectSet ef, List<string> h)
        {
            if (ci == null)
            {
                return;
            }
            Hand.Remove(ci);
            Exhaust(ci, ef);
            h.Add($"Exhausted {ci}");
        }

        /// <summary>
        /// for the cards we know we will draw, draw them from drawpile.
        /// If drawpile runs out, reshuffle and do it again.
        /// </summary>
        internal void ForceDrawCards(Player player, IList<CardInstance> initialHand, EffectSet ef, List<string> history)
        {
            var res = new List<CardInstance>();
            //if (HasNoDrawStatus(player))
            //{
            //    history.Add("No drawing due to nodraw status.");
            //    return;
            //}
            var found = new List<CardInstance>();
            var initialCopy = initialHand.Select(el => el.Copy()).ToList();
            foreach (var ci in initialCopy)
            {
                var copy = FindIdenticalCardInSource(DrawPile, ci, failureOkay: true);

                if (copy != null)
                {
                    DrawPile.Remove(copy);
                    res.Add(copy);
                    found.Add(ci);
                }
            }
            foreach (var f in found)
            {
                initialCopy.Remove(f);
            }

            if (initialCopy.Any())
            {
                Reshuffle(ef, history);
                foreach (var ci in initialCopy)
                {
                    var copy = FindIdenticalCardInSource(DrawPile, ci, failureOkay: false);
                    DrawPile.Remove(copy);
                    res.Add(copy);
                }
            }
            var handCopy = Hand.Select(el => el.Copy()).ToList();
            handCopy.AddRange(res);
            Hand = handCopy;
        }

        /// <summary>
        /// Figure out what we would draw if we really did.  Ahd it'd be better to really find them.
        /// </summary>
        public IList<CardInstance> WouldDraw(int drawCount)
        {
            if (_PreserveOrder)
            {

                var forceRes = DrawPile.Skip(DrawPile.Count() - drawCount).Take(drawCount).ToList();
                if (forceRes.Count < drawCount)
                {
                    _PreserveOrder = false;
                    //just nuke the state and do a normal draw.  You'll get the initial cards left in draw plus random ones from the discard pile.
                    return WouldDraw(drawCount);
                }
                return forceRes;
            }

            var res = new List<CardInstance>();
            var drawCopy = DrawPile.Select(el => el.Copy()).ToList();
            var discardCopy = DiscardPile.Select(el => el.Copy()).ToList();

            int toDraw = drawCount;
            while (toDraw > 0)
            {
                if (drawCopy.Count > 0)
                {
                    var ii = Rnd.Next(drawCopy.Count);
                    var el = drawCopy[ii];
                    drawCopy.Remove(el);
                    res.Add(el);
                }
                else
                {
                    if (discardCopy.Count == 0)
                    {
                        //can't do a full draw.

                        break;
                    }
                    var ii = Rnd.Next(discardCopy.Count);
                    var el = discardCopy[ii];
                    discardCopy.Remove(el);
                    res.Add(el);
                }
                toDraw--;
            }

            return res;
        }

        public List<CardInstance> DrawCards(int drawCount, EffectSet ef, List<string> history)
        {
            var actuallyDrawn = new List<CardInstance>();
            Hand = new List<CardInstance>();

            IList<CardInstance> which;
            which = WouldDraw(drawCount);

            var gap = drawCount - which.Count;

            if (gap > 0)
            {
                history.Add($"No cards left to draw {gap} more");
            }

            foreach (var copy in which)
            {
                var orig = FindIdenticalCardInSource(DrawPile, copy, failureOkay: true);
                if (orig != null && DrawPile.Contains(orig))
                {
                    DrawPile.Remove(orig);
                    TryAddToHand(orig, ef);
                    actuallyDrawn.Add(orig);
                }
                else
                {
                    Reshuffle(ef, history); //this should only happen 0-1 times
                    orig = FindIdenticalCardInSource(DrawPile, copy, failureOkay: false);
                    if (DrawPile.Contains(orig))
                    {
                        DrawPile.Remove(orig);
                        TryAddToHand(orig, ef);
                        actuallyDrawn.Add(orig);
                    }
                    else
                    {
                        throw new Exception("Card missing after reshuffle");
                    }
                }
            }
            return actuallyDrawn;
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

        internal void AfterPlayingCard(CardInstance ci, EffectSet ef, List<string> history)
        {
            if (ci.Card.CardType == CardType.Power)
            {
                //card just disappears.
            }
            else
            {
                ci.LeavingHand(ef);
                if (ci.Exhausts())
                {
                    Exhaust(ci, ef);
                    history.Add($"Exhausted {ci}");
                }
                else
                {
                    PutIntoDiscardAfterApplyingEffectSet = ci;
                }
            }
        }

        internal void CardPlayCleanup()
        {
            if (PutIntoDiscardAfterApplyingEffectSet != null)
            {
                DiscardPile.Add(PutIntoDiscardAfterApplyingEffectSet);
                PutIntoDiscardAfterApplyingEffectSet = null;
            }
        }

        private CardInstance PutIntoDiscardAfterApplyingEffectSet { get; set; }

        public void Reshuffle(EffectSet ef, List<string> history)
        {
            DeckShuffle?.Invoke(ef, history);
            foreach (var d in DiscardPile)
            {
                DrawPile.Add(d);
            }

            DiscardPile = new List<CardInstance>();
            ShuffleDrawPile();
            ef.HadRandomness = true;
            ef.Key = GenerateCardSetKey(DrawPile);
        }

        /// <summary>
        /// When we reshuffle, we mark the shuffle order with this key for disambiguation in the fightnode tree.
        /// </summary>
        public long GenerateCardSetKey(IList<CardInstance> cis)
        {
            var strings = cis.Select(el => el.ToString());
            var combined = SJ(strings);
            var bytes = System.Text.Encoding.ASCII.GetBytes(combined);
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(bytes);
                var res = BitConverter.ToInt64(hash);
                return res;
            }
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

            d._PreserveOrder = _PreserveOrder;

            //it is pretty bad we have to copy event connections.
            d.DeckShuffle = DeckShuffle;
            d.DrawCard = DrawCard;
            d.ExhaustCard = ExhaustCard;
            d.InteractiveContext = InteractiveContext;

            return d;
        }

        /// <summary>
        /// Adds to a random spot in the draw pile
        /// </summary>
        internal void AddToDrawPile(CardInstance newCi)
        {
            //assuming we want to interleave it.
            DrawPile.Add(newCi);
        }

        /// <summary>
        /// Returns position key.
        /// </summary>
        internal long AddToRandomSpotInDrawPile(CardInstance ci, long? key = null)
        {
            long position;
            if (key == null)
            {
                position = Rnd.Next(DrawPile.Count + 1);
            }
            else
            {
                position = key.Value;
            }
            DrawPile.Insert((int)position, ci);
            return position;
        }

        internal void AddToDiscardPile(CardInstance newCi)
        {
            //assuming we want to interleave it.
            DiscardPile.Add(newCi);
        }
    }
}
