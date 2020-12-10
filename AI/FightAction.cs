using System.Collections.Generic;

namespace StS
{


    /// <summary>
    /// play a card, endTurn, drink potion
    /// Expand this to have a description of what happened.
    /// </summary>
    public class FightAction
    {
        public FightActionEnum FightActionType { get; private set; }
        public Potion Potion { get; private set; }
        public CardInstance Card { get; private set; }
        public List<string> Desc { get; private set; }
        public IEntity Enemy { get; private set; }

        public FightAction(FightActionEnum fightActionType, Potion potion = null, CardInstance card = null, IEntity enemy = null, List<string> desc = null)
        {
            FightActionType = fightActionType;
            Potion = potion?.Copy();
            Card = card?.Copy();
            Enemy = enemy;
            Desc = desc;
        }

        internal FightAction Copy()
        {
            throw new System.Exception("Don't copy this");
            //var ah = new FightAction(FightActionType, Potion?.Copy(), Card?.Copy(), Enemy, Desc);
            //return ah;
        }

        public override string ToString()
        {
            string label = "* " + FightActionType.ToString();
            var forceIncludeLabel = false;
            var extra = "";

            //Certain types are always labelled
            switch (FightActionType)
            {
                case FightActionEnum.PlayCard:
                    label = $"* {Card}";
                    forceIncludeLabel = true;
                    break;
                case FightActionEnum.Potion:
                    label = "* Potion:" + Potion.ToString();
                    forceIncludeLabel = true;
                    break;
                case FightActionEnum.EnemyDied:
                    forceIncludeLabel = true;
                    label = $"* Enemy {Enemy} died";
                    break;
                case FightActionEnum.StartTurn:
                case FightActionEnum.EndTurn:
                case FightActionEnum.WonFight:
                case FightActionEnum.LostFight:
                case FightActionEnum.TooLong:
                case FightActionEnum.EnemyAttack:
                case FightActionEnum.EnemyBuff:
                case FightActionEnum.EnemyStatusAttack:
                    forceIncludeLabel = true;
                    break;
                case FightActionEnum.StartFight:
                    //extra = "\n";
                    forceIncludeLabel = true;
                    break;
                case FightActionEnum.StartFightEffect:
                case FightActionEnum.EndFightEffect:

                case FightActionEnum.StartTurnEffect:
                case FightActionEnum.EndTurnEffect:

                case FightActionEnum.EndTurnDeckEffect:
                case FightActionEnum.EndTurnOtherEffect:
                    break;
                default:
                    throw new System.Exception();
            }

            if (!forceIncludeLabel && Desc.Count == 0)
            {
                return "";
            }

            var descPart = "";
            if (Desc?.Count > 0)
            {
                descPart = $" {string.Join(" ", Desc)}";
            }

            return $"{extra}{label}{descPart}";
        }
    }
}
