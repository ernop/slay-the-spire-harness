﻿using System;

namespace StS
{
    public class StatusInstance
    {
        /// <summary>
        /// What kinda status is this?
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// Extract these from num on creation
        /// </summary>
        public int Intensity { get; set; }
        public int Duration { get; set; }
        //public Entity Parent { get; set; }

        /// <summary>
        /// quite bad that intensity needs to be hardcoded when for some statuses it's actually just fixed.
        /// </summary>
        public StatusInstance(Status status, int num)
        {
            //Parent = parent ?? throw new ArgumentNullException("Parent");
            Status = status;
            if (status.Scalable)
            {
                Intensity = num;
                if (status.Permanent)
                {
                    Duration = int.MaxValue; //str
                }
                else
                {
                    Duration = 1; //flamebarrier, penNibDD
                }
            }
            else
            {
                Intensity = 1;
                if (status.Permanent)
                {
                    Duration = int.MaxValue; //will be taken care of with a temporary cancelling status.
                }
                else
                {
                    Duration = num; //vuln
                    //pennib
                }
            }
        }

        internal void FirstTurn(Entity parent, EffectSet firstTurnPlayerEf)
        {
            throw new NotImplementedException();
        }

        public void EndTurn(Entity parent, EffectSet endTurnEf)
        {
            if (!Status.Permanent)
            {
                Duration--;
            }
            //remove them externally since we're iterating here.
            if (Duration < 0)
            {
                throw new Exception("Negative Duration");
            }

            Status.EndTurn(parent, this, endTurnEf);
        }

        public override string ToString()
        {
            string explanation;
            if (Status.Permanent)
            {
                explanation = " I:" + Intensity; //str
            }
            else
            {
                if (Status.Scalable)
                {
                    explanation = " I:" + Intensity; //flame barrier
                }
                else
                {
                    explanation = " D:" + Duration;
                }
            }

            return $"{Status.Name}{explanation}";
        }

        public void Apply(Card card, IndividualEffect sourceSet, IndividualEffect targetSet, bool statusIsTargeted, bool playerAction)
        {
            Status.Apply(card, sourceSet, targetSet, Intensity, statusIsTargeted, playerAction);
        }
    }
}
