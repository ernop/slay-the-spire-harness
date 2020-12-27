using System;
using System.Collections.Generic;
using System.Linq;

using static StS.Helpers;

namespace StS
{
    /// <summary>
    /// Should I iterate over all possibilities as deep as possible?
    /// How much will things actually blow up?  With fully unique deck (no repeat cards)
    /// 
    /// How about just normal play and do lots of sims vs a dumb simple enemy without.
    /// 
    /// TODO: it'd be beter
    /// </summary>
    public class MonteCarlo
    {
        private Player _Player { get; set; }
        private Enemy _Enemy { get; set; }
        private Deck _Deck {get;set;}
        
        /// <summary>
        /// for overriding
        /// </summary>
        private int _TurnNumber { get; set; }
        public static int MCCount { get; set; } = 0;
        public FightNode Root { get; set; }
        private FightAction _FirstAction { get; set; }

        /// <summary>
        /// previously this exhaustively simulated the entire fight til turnnumber.
        /// But now it will just do a bunch of MCs.
        /// Hopefully this will be enough to find the right solution.
        /// </summary>
        public FightNode SimAfterFirstDraw(int? n = 10000)
        {
            for (var ii = 0; ii < n; ii++)
            {
                MC(Root.Randoms.First());
            }
            return Root;
        }

        public FightNode SimIncludingDraw(int? n = 10000)
        {
            for (var ii = 0; ii < n; ii++)
            {
                MC(Root);
            }
            return Root;
        }

        public MonteCarlo(Deck deck, Enemy enemy, Player player, int turnNumber = 0, List<string> firstHand = null)
        {
            _Deck = deck ?? throw new ArgumentNullException(nameof(deck));
            _Enemy = enemy ?? throw new ArgumentNullException();
            _Player = player ?? throw new ArgumentNullException();
            _TurnNumber = turnNumber;

            if (firstHand != null)
            {
                var cards = _Deck.FindSetOfCards(_Deck.GetDrawPile, firstHand);
                var firstAction = new FightAction(FightActionEnum.StartTurn, cards);
                _FirstAction = firstAction;
            }            

            var fight = new Fight(_Deck, _Player, _Enemy);
            
            fight.TurnNumber = _TurnNumber;

            var root = new FightNode(fight);
            fight.FightNode = root;
            root.StartFight();
            Root = root;
            if (_FirstAction != null)
            {
                root.ApplyAction(_FirstAction);
            }
        }

        /// <summary>
        /// Returns the leaf node of a single mc run.
        /// </summary>
        public FightNode MC(FightNode fn)
        {
            if (fn.Depth == 1)
            {
                MCCount++;
            }
            var actions = fn.Fight.GetAllActions();
          
            var ii = Rnd.Next(actions.Count());
            var action = actions[ii];

            var childNode = fn.ApplyAction(action);

            switch (childNode.Fight.Status)
            {
                case FightStatus.Ongoing:
                    return MC(childNode);
                case FightStatus.Won:
                    return childNode;
                case FightStatus.Lost:
                    return childNode;
                default:
                    throw new Exception("Other status");
            }
        }
    }
}