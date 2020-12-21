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
        private IList<CardInstance> _CIs { get; set; }
        private IList<CardInstance> _FirstHand { get; set; }
        private Deck _Deck {get;set;}
        
        /// <summary>
        /// for overriding
        /// </summary>
        private int _TurnNumber { get; set; }

        /// <summary>
        /// Records the actual state of the fight and runs sims to make a good decision.
        /// </summary>
        public MonteCarlo(IList<CardInstance> cis, IList<CardInstance> firstHand, Enemy enemy, Player player)
        {
            _CIs = cis ?? throw new ArgumentNullException(nameof(cis));
            _FirstHand = firstHand ?? throw new ArgumentNullException(nameof(firstHand));
            _Enemy = enemy ?? throw new ArgumentNullException();
            _Player = player ?? throw new ArgumentNullException();
        }

        public MonteCarlo(Deck deck, IList<CardInstance> firstHand, Enemy enemy, Player player, int turnNumber)
        {
            _Deck = deck ?? throw new ArgumentNullException(nameof(deck));
            _FirstHand = firstHand ?? throw new ArgumentNullException(nameof(firstHand)); //no need to
            _Enemy = enemy ?? throw new ArgumentNullException();
            _Player = player ?? throw new ArgumentNullException();
            _TurnNumber = turnNumber;
        }

        /// <summary>
        /// Returns a list of fightnode roots based on initial draws.
        /// TODO add "randomchoices" and create a test for pommel strike.
        /// SS PS DD is your draw
        /// </summary>
        public FightNode Sim()
        {
            Fight fight;
            if (_Deck == null) //magic override
            {
                fight = new Fight(_CIs, _Player, _Enemy);
            }
            else
            {
                fight = new Fight(_Deck, _Player, _Enemy);
            }
            fight.TurnNumber = _TurnNumber;

            // TODO: future - weigh by frequency
            //var count = item.Item2;

            var root = new FightNode(fight);
            fight.FightNode = root;
            root.StartFight(_FirstHand);

            MC(root);
            return root;
        }

        public static int MCCount { get; set; } = 0;

        public void MC(FightNode fn)
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
                    MC(childNode);
                    break;
                case FightStatus.Won:
                    break;
                case FightStatus.Lost:
                    break;
                default:
                    throw new Exception("Other status");
            }
        }

        /// <summary>
        /// actually an application of an action can result in multiple child nodes.
        /// For example: if you play battle trance, the child nodes are all the possible ways to draw.
        /// </summary>
        private FightNode ApplyAction(FightNode fn, FightAction action)
        {
            //check if a child already has this action; if so just return that one.
            //would be a lot better to just MC the child directly rather than MCing the parent and then redoing this traversal.

            foreach (var c in fn.Choices)
            {
                if (c.FightAction.AreEqual(action))
                {
                    return c;
                }
            }
            foreach (var r in fn.Randoms)
            {
                var f = r.FightAction.Card?.ToString();
                if (r.FightAction.AreEqual(action))
                {
                    return r;
                }
                else
                {
                    var ae = 4;
                    //return fn;
                }
            }

            switch (action.FightActionType)
            {
                case FightActionEnum.PlayCard:
                    var child = fn.PlayCard(action.Card); //non-rnd
                    return child;
                case FightActionEnum.Potion:
                    var child2 = fn.DrinkPotion(action.Potion, _Enemy); //non-rnd
                    return child2;
                case FightActionEnum.EndTurn:
                    var child3 = fn.EndTurn(); //non-rnd
                    return child3;
                case FightActionEnum.EnemyMove:
                    var child4 = fn.EnemyMove(action); //rnd
                    return child4;
                case FightActionEnum.StartTurn:
                    var child5 = fn.StartTurn(action.CardsDrawn);
                    return child5;
                default:
                    throw new Exception("Invalid action");
            }
        }
    }
}