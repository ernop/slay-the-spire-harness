﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace StS
{


    /// <summary>
    /// Player did something; then the list of accumulated actions.
    /// </summary>
    public class FightAction// : IEqualityComparer<FightAction>
    {
        public FightActionEnum FightActionType { get; private set; }
        public Potion Potion { get; private set; }
        public CardInstance Card { get; private set; }
        public List<string> History { get; private set; }
        public IEntity Target { get; private set; }
        
        /// <summary>
        /// Whether this was a random action at generation point.
        /// </summary>
        public bool Random { get; set; }
        
        /// <summary>
        /// The key to disambiguate actions which are otherwise similar but involve randomness.
        /// e.g. true grit exhausts one random card. So every time it's played in the same situation,
        /// if there are 4 cards still in hand, there would be version with key 0-4
        /// Similarly, wild strike would have N+1 where N is the number of cards in the draw pile.
        /// </summary>
        public int? Key { get; set; }

        /// <summary>
        /// either StartTurn in which case they represent cards drawn
        /// or "true grit" + some target to designate which is exhausted
        /// </summary>
        public IList<CardInstance> CardTargets { get; set; }

        /// <summary>
        /// an action in a fight is one of:
        /// * player play card
        /// * player drink pot
        /// * player end turn
        /// 
        /// * enemy play card (enemyAttack)
        /// * enemy buff (or do nothing)
        /// * enemy playerStatusATtack
        /// </summary>
        public FightAction(FightActionEnum fightActionType, IList<CardInstance> cardsDrawn = null, Potion potion = null, CardInstance card = null,
            IEntity target = null, List<string> history = null, int? key = null)
        {
            CardTargets = cardsDrawn;
            FightActionType = fightActionType;
            Potion = potion?.Copy();
            Card = card?.Copy();
            Target = target;
            History = history;
            Key = key;
            switch (fightActionType)
            {
                case FightActionEnum.Potion:
                    Random = potion.Random;
                    break;
                case FightActionEnum.PlayCard:
                case FightActionEnum.EndTurn:
                case FightActionEnum.StartTurnEffect:
                case FightActionEnum.EndTurnEffect:
                case FightActionEnum.EndTurnDeckEffect:
                case FightActionEnum.EndTurnOtherEffect:
                case FightActionEnum.StartFightEffect:
                case FightActionEnum.EndFightEffect:
                case FightActionEnum.EnemyDied:
                case FightActionEnum.EndEnemyTurn:
                case FightActionEnum.StartFight:
                case FightActionEnum.WonFight:
                case FightActionEnum.LostFight:
                case FightActionEnum.TooLong:
                case FightActionEnum.NotInitialized:
                    Random = false;
                    break;
                case FightActionEnum.StartTurn:
                case FightActionEnum.EnemyMove:
                    Random = true;
                    break;
            }
            Validate();
        }

        private void Validate()
        {
            switch (FightActionType)
            {
                case FightActionEnum.PlayCard:
                    if (Card == null) throw new InvalidOperationException();
                    if (Target != null) throw new InvalidOperationException();
                    break;
                case FightActionEnum.Potion:
                    if (Potion == null) throw new InvalidOperationException();
                    if (Target == null) throw new InvalidOperationException();
                    break;
                case FightActionEnum.EndTurn:
                    break;
                case FightActionEnum.StartTurn:
                    //if (CardTargets == null) throw new InvalidOperationException();
                    break;
                case FightActionEnum.StartTurnEffect:
                    break;
                case FightActionEnum.EndTurnEffect:
                    break;
                case FightActionEnum.EndTurnDeckEffect:
                    break;
                case FightActionEnum.EndTurnOtherEffect:
                    break;
                case FightActionEnum.StartFightEffect:
                    break;
                case FightActionEnum.EndFightEffect:
                    break;
                case FightActionEnum.EnemyMove:
                    break;
                case FightActionEnum.EnemyDied:
                    break;
                case FightActionEnum.EndEnemyTurn:
                    break;
                case FightActionEnum.StartFight:
                    break;
                case FightActionEnum.WonFight:
                    break;
                case FightActionEnum.LostFight:
                    break;
                case FightActionEnum.TooLong:
                    break;
                case FightActionEnum.NotInitialized:
                    break;
            }
        }

        internal FightAction Copy()
        {
            throw new System.Exception("Don't copy this");
        }

        public override string ToString()
        {
            var label = FightActionType.ToString();

            //Certain types are always labelled
            switch (FightActionType)
            {
                case FightActionEnum.PlayCard:
                    label = $"{Card}";
                    break;
                case FightActionEnum.Potion:
                    label = "Potion:" + Potion.ToString();
                    break;
                case FightActionEnum.EnemyDied:
                    label = $"Enemy {Target} died";
                    break;
                case FightActionEnum.StartTurn:
                    break;
                case FightActionEnum.EndTurn:
                case FightActionEnum.WonFight:
                case FightActionEnum.LostFight:
                case FightActionEnum.TooLong:
                    break;
                case FightActionEnum.EnemyMove:
                    break;
                case FightActionEnum.StartFight:
                    break;
                case FightActionEnum.StartFightEffect:
                case FightActionEnum.EndFightEffect:

                case FightActionEnum.StartTurnEffect:
                case FightActionEnum.EndTurnEffect:

                case FightActionEnum.EndTurnDeckEffect:
                case FightActionEnum.EndTurnOtherEffect:
                case FightActionEnum.EndEnemyTurn:
                    break;
                case FightActionEnum.NotInitialized:
                    break;
                default:
                    throw new System.Exception();
            }


            //we always return a fighthistory.

            var descPart = "";
            if (History?.Count > 0)
            {
                descPart = $" {string.Join(" ", History)}";
            }
            var rnd = "";
            if (Random)
            {
                rnd = $" R:{Key}";
            }

            return $"{label,-15}{rnd}{descPart}";
        }

        /// <summary>
        /// When we generate an action, we check to see if it's already a duplicate (in randoms or choices).  This is how we compare them.
        /// </summary>
        public bool IsEqual(FightAction other)
        {
            if (FightActionType == other.FightActionType)
            {
                if (Card?.ToString() == other.Card?.ToString()) //we compare cards including enemy attacks by ToString
                {
                    if (Potion?.Name == other.Potion?.Name)
                    {
                        if (Target?.Name == other.Target?.Name)
                        {
                            if (CardTargets == null && other.CardTargets == null)
                            { return true; }
                            if (Helpers.CompareHands(CardTargets, other.CardTargets, out var msg))
                            {
                                if (Key == other.Key)
                                {
                                    if (Random == false)
                                    {
                                        return true;
                                    }
                                    
                                    //random action with same key.
                                    if (Key == other.Key)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        internal void AddHistory(List<string> history)
        {
            if (History == null) History = new List<string>();
            History.AddRange(history);
        }
    }
}
