using System;
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
        public List<string> Desc { get; private set; }
        public IEntity Target { get; private set; }
        public IList<CardInstance> CardsDrawn { get; set; }

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
            IEntity target = null, List<string> desc = null)
        {
            CardsDrawn = cardsDrawn;
            FightActionType = fightActionType;
            Potion = potion?.Copy();
            Card = card?.Copy();
            Target = target;
            Desc = desc;
        }

        internal FightAction Copy()
        {
            throw new System.Exception("Don't copy this");
        }

        public override string ToString()
        {
            var label = FightActionType.ToString();
            bool forceIncludeLabel;
            var extra = "";

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
                    label = $"EnemyAction {Card}";
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
                default:
                    throw new System.Exception();
            }


            //we always return a fighthistory.

            var descPart = "";
            if (Desc?.Count > 0)
            {
                descPart = $" {string.Join(" ", Desc)}";
            }

            return $"{extra}{label}{descPart}";
        }

        public bool AreEqual(FightAction other)
        {
            if (FightActionType == other.FightActionType)
            {
                if (Card?.ToString() == other.Card?.ToString()) //we compare cards including enemy attacks by ToString
                {
                    if (Potion == other.Potion)
                    {
                        if (Target == other.Target)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //public int GetHashCode([DisallowNull] FightAction obj)
        //{

        //    var hashed = hasher.ComputeHash(Encoding.UTF8.GetBytes(obj.ToString()));
        //    var ivalue = BitConverter.ToInt32(hashed, 0);
        //    return ivalue;
        //}

        //public static MD5 hasher = MD5.Create();
    }
}
