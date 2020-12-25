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
        public FightNode SimAfterFirstDraw()
        {
            for (var ii = 0; ii < 10000; ii++)
            {
                MC(Root.Randoms.First());
            }
            return Root;
        }

        public FightNode SimIncludingDraw()
        {
            for (var ii = 0; ii < 10000; ii++)
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
                ApplyAction(root, _FirstAction);
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

            var childNode = ApplyAction(fn, action);

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

        /// <summary>
        /// if from node n you choose an action which is already in choices,
        /// or do a draw which is already in randoms,
        /// just return that child, rather than actually applying action on our current copy again.
        /// </summary>
        private FightNode FindDuplicate(FightNode fn, FightAction action)
        {
            foreach (var c in fn.Choices)
            {
                if (c.FightAction.IsEqual(action))
                {
                    return c;
                }
            }
            foreach (var r in fn.Randoms)
            {
                var f = r.FightAction.Card?.ToString();
                if (r.FightAction.IsEqual(action))
                {
                    return r;
                }
            }
            return null;
        }

        /// <summary>
        /// actually an application of an action can result in multiple child nodes.
        /// For example: if you play battle trance, the child nodes are all the possible ways to draw.
        /// </summary>
        private FightNode ApplyAction(FightNode fn, FightAction action)
        {
            //check if a child already has this action; if so just return that one.
            //would be a lot better to just MC the child directly rather than MCing the parent and then redoing this traversal.

            var dup = FindDuplicate(fn, action);
            if (dup != null)
            {
                dup.Weight++;
                return dup;
            }

            switch (action.FightActionType)
            {
                case FightActionEnum.PlayCard:
                    var child = fn.PlayCard(action); //non-rnd
                    return child;
                case FightActionEnum.Potion:
                    var child2 = fn.DrinkPotion(action); //non-rnd
                    return child2;
                case FightActionEnum.EndTurn:
                    var child3 = fn.EndTurn(); //non-rnd
                    return child3;
                case FightActionEnum.EnemyMove:
                    var child4 = fn.EnemyMove(action); //rnd
                    return child4;
                case FightActionEnum.StartTurn:
                    var child5 = fn.StartTurn(action);
                    return child5;
                default:
                    throw new Exception("Invalid action");
            }
        }
    }
}