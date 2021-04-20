using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
        public CardInstance CardInstance { get; private set; }
        public List<string> History { get; set; }
        public IEntity Target { get; private set; }
        public bool Playable { get; set; }

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
        public long? Key { get; set; }

        /// <summary>
        /// for generic actions when iterating over possibilities, there are multiple keys.
        /// </summary>
        public List<long> Keys { get; set; }

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
        public FightAction(FightActionEnum fightActionType, CardInstance card = null, IList<CardInstance> cardTargets = null, Potion potion = null,
            IEntity target = null, List<string> history = null, long? key = null, List<long> keys = null, bool hadRandomEffects = false, bool playable = true)
        {
            CardTargets = cardTargets;
            FightActionType = fightActionType;
            Potion = potion?.Copy();
            CardInstance = card?.Copy();
            Target = target;
            History = history;
            Keys = keys;
            Key = key;
            Random = hadRandomEffects;

            // For console view, whether it can actually be chosen/played.
            Playable = playable;
            switch (fightActionType)
            {
                case FightActionEnum.Potion:
                    if (hadRandomEffects != potion.Random) throw new Exception("Enemy moves always random.");
                    break;
                case FightActionEnum.PlayCard:
                    if (hadRandomEffects && !card.Card.RandomEffects) throw new Exception("Unexpected");
                    //had => rE but RE !=> had since not every randomizable effect card will actually have an effect every time.
                    break;
                case FightActionEnum.EndTurn:
                case FightActionEnum.StartTurnEffect:
                case FightActionEnum.EndTurnEffect:
                case FightActionEnum.EndTurnDeckEffect:
                case FightActionEnum.EndTurnOtherEffect:
                case FightActionEnum.StartFightEffect:
                case FightActionEnum.EndFightEffect:
                case FightActionEnum.EnemyDied:
                case FightActionEnum.EndEnemyTurn:

                case FightActionEnum.WonFight:
                case FightActionEnum.LostFight:
                case FightActionEnum.TooLong:
                case FightActionEnum.NotInitialized:
                case FightActionEnum.StartTurn:
                    if (hadRandomEffects) throw new Exception($"{fightActionType} is not random since deck was already shuffled.");
                    break;
                case FightActionEnum.StartFight:
                    if (!hadRandomEffects) throw new Exception($"StartFight should be random.");
                    break;
                case FightActionEnum.EnemyMove:
                    if (!hadRandomEffects) throw new Exception("Enemy moves always random.");
                    //I need a key to disambiguate the same actions.
                    //i.e. if a monster has two choices that would be key 0 and key 1.
                    //harder is to find a way to handle draws. ideally it'd be hand.GetHash()
                    break;
            }

            Validate();
        }

        private void Validate()
        {
            switch (FightActionType)
            {
                case FightActionEnum.PlayCard:
                    if (CardInstance == null) throw new InvalidOperationException();
                    if (Target != null) throw new InvalidOperationException();
                    break;
                case FightActionEnum.Potion:
                    if (Potion == null) throw new InvalidOperationException();
                    if (Target == null) throw new InvalidOperationException();
                    break;
                case FightActionEnum.EndTurn:
                case FightActionEnum.StartTurn:
                case FightActionEnum.StartTurnEffect:
                case FightActionEnum.EndTurnEffect:
                case FightActionEnum.EndTurnDeckEffect:
                case FightActionEnum.EndTurnOtherEffect:
                case FightActionEnum.StartFightEffect:
                case FightActionEnum.EndFightEffect:
                case FightActionEnum.EnemyMove:
                case FightActionEnum.EnemyDied:
                case FightActionEnum.EndEnemyTurn:
                case FightActionEnum.StartFight:
                case FightActionEnum.WonFight:
                case FightActionEnum.LostFight:
                case FightActionEnum.TooLong:
                case FightActionEnum.NotInitialized:
                    break;
            }
        }

        internal FightAction Copy()
        {
            throw new System.Exception("Don't copy this");
        }



        /// <summary>
        /// When we generate an action, we check to see if it's already a duplicate (in randoms or choices).  This is how we compare them.
        /// </summary>
        public bool IsEqual(FightAction other)
        {
            if (FightActionType != other.FightActionType) return false;
            if (CardInstance?.ToString() != other.CardInstance?.ToString()) return false;
            if (Potion?.Name != other.Potion?.Name) return false;
            if (Target?.Name != other.Target?.Name) return false;
            if (CardTargets != null && other.CardTargets == null) return false;
            if (CardTargets == null && other.CardTargets != null) return false;
            if (!Helpers.CompareHands(CardTargets, other.CardTargets, out var msg2)) return false;
            //if (Key != other.Key) return false;
            //we do not compare key here because this is at the "choice" stage.
            if (Random != other.Random) return false;

            return true;
        }

        internal void AddHistory(List<string> history)
        {
            if (History == null) History = new List<string>();
            History.AddRange(history);
        }

        public override string ToString()
        {
            var historyParts = GetList();
            var descPart = string.Join(" ", historyParts.Skip(1));
            return $"{historyParts[0],-15}{descPart}";
        }

        public List<string> GetList()
        {
            var label = FightActionType.ToString();

            //Certain types are always labelled
            switch (FightActionType)
            {
                case FightActionEnum.PlayCard:
                    label = $"{CardInstance}";
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
            var res = new List<string>() { label };

            if (History != null)
            {
                res.AddRange(History);
            }
            if (Random)
            {
                //possibly unnecessary in interactive context.
                res.Add($" R:{Key}");
            }

            return res;
        }
    }
}
