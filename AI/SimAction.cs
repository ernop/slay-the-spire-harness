using System;

namespace StS
{


    /// <summary>
    /// play a card, endTurn, drink potion
    /// </summary>
    public class SimAction
    {
        public SimActionEnum SimActionType { get; set; }
        public Potion Potion { get; set; }
        public CardInstance Card { get; set; }

        public SimAction(SimActionEnum simActionType, Potion potion, CardInstance card)
        {
            SimActionType = simActionType;
            Potion = potion?.Copy();
            Card = card?.Copy();
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
                case SimActionEnum.Card:
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
