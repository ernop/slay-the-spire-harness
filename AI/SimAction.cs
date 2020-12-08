using System.Collections.Generic;

namespace StS
{


    /// <summary>
    /// play a card, endTurn, drink potion
    /// Expand this to have a description of what happened.
    /// </summary>
    public class SimAction
    {
        public SimActionEnum SimActionType { get; private set; }
        public Potion Potion { get; private set; }
        public CardInstance Card { get; private set; }
        public List<string> Desc { get; private set; }

        public SimAction(SimActionEnum simActionType, Potion potion = null, CardInstance card = null, List<string> desc = null)
        {
            SimActionType = simActionType;
            Potion = potion?.Copy();
            Card = card?.Copy();
            Desc = desc ?? new List<string>();
        }

        internal SimAction Copy()
        {
            var ah = new SimAction(SimActionType, Potion?.Copy(), Card?.Copy());
            return ah;
        }

        public override string ToString()
        {
            string desc;
            switch (SimActionType)

            {
                case SimActionEnum.PlayCard:
                    desc = $"Play {Card}";
                    break;
                case SimActionEnum.Potion:
                    desc = "Potion:" + Potion.ToString();
                    break;
                case SimActionEnum.EndTurn:
                    desc = "End Turn";
                    break;
                default:
                    throw new System.Exception();
            }
            return $"Action: {desc}";
        }
    }
}
