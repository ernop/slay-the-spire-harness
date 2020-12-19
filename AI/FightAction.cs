using System.Collections.Generic;

namespace StS
{


    /// <summary>
    /// Player did something; then the list of accumulated actions.
    /// </summary>
    public class FightAction
    {
        public FightActionEnum FightActionType { get; private set; }
        public Potion Potion { get; private set; }
        public CardInstance Card { get; private set; }
        public List<string> Desc { get; private set; }
        public IEntity Target { get; private set; }

        public FightAction(FightActionEnum fightActionType, Potion potion = null, CardInstance card = null, IEntity target = null, List<string> desc = null)
        {
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
                    label = $"* Enemy {Target} died";
                    break;
                case FightActionEnum.StartTurn:
                    //label = $"Start {Fight}";
                    forceIncludeLabel = true;
                    break;
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
    }
}
