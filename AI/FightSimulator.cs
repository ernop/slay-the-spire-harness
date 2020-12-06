using System;
using System.Collections.Generic;

namespace StS
{

    /// <summary>
    /// just keep copying the fight playing out every choice.
    /// All this copying of the fight is expensive!
    /// </summary>


    /// <summary>
    /// Should I iterate over all possibilities as deep as possible?
    /// How much will things actually blow up?  With fully unique deck (no repeat cards)
    /// 
    /// How about just normal play and do lots of sims vs a dumb simple enemy without.
    /// </summary>
    public class FightSimulator
    {
        private Player _Player { get; set; }
        private Enemy _Enemy { get; set; }
        private List<CardInstance> _CIs { get; set; }

        /// <summary>
        /// Records the actual state of the fight and runs sims to make a good decision.
        /// </summary>
        public FightSimulator(List<CardInstance> cis, Enemy enemy, Player player)
        {
            _CIs = cis;
            _Enemy = enemy;
            _Player = player;
        }

        public void Sim()
        {

            var fight = new Fight(_CIs, _Player, _Enemy);
            fight.StartTurn();
            var res = Iter(fight);

        }

        private List<Fight> Iter(Fight fight)
        {
            var actions = fight.GetAllActions();
            Console.WriteLine($"Iter fight with action length: {fight.ActionHistory.Count}");
            var res = new List<Fight>();
            if (fight.ActionHistory.Count > 50)
            {
                Console.WriteLine("Break");
                return res;
            }
            foreach (var action in actions)
            {
                var fc = fight.Copy();
                var finishedFight = ApplyAction(fc, action);
                if (finishedFight == null)
                {
                    Iter(fc);
                }
                else
                {
                    res.Add(fc);
                }
            }
            return res;
        }

        /// <summary>
        /// returns fight when reached the end.
        /// </summary>
        private Fight ApplyAction(Fight fight, SimAction action)
        {
            fight.ActionHistory.Add(action);
            switch (action.SimActionType)
            {
                case SimActionEnum.Card:
                    Console.WriteLine($"{action.Card}");
                    var copiedCard = fight.FindIdenticalCard(action.Card);
                    fight.PlayCard(copiedCard);
                    break;
                case SimActionEnum.EndTurn:
                    Console.WriteLine($"End Turn");
                    fight.EndTurn();
                    //monsterTurn

                    fight.EnemyMove();
                    fight.StartTurn();
                    break;
                case SimActionEnum.Potion:
                    Console.WriteLine($"Drink {action.Potion}");
                    _Player.DrinkPotion(fight, action.Potion, _Enemy);
                    break;
                default:
                    break;
            }
            if (fight.Status == FightStatus.Ongoing)
            {
                return null;
            }
            else
            {
                return fight;
            }
        }
    }
}