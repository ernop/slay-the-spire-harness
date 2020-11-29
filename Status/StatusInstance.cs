using System;

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

        internal StatusInstance Copy()
        {
            return new StatusInstance(Status, GetCurrentStatusNum());
        }

        /// <summary>
        /// read out the current way to recreate the status.
        /// </summary>
        /// <returns></returns>
        private int GetCurrentStatusNum()
        {
            int num;
            if (Status.Scalable)
            {
                num = Intensity;
            }
            else
            {
                num = Duration;
            }
            return num;
        }


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
                    //this is for statuses that are permanent and nonscalable.  What is an example of them?
                }
                else
                {
                    Duration = num; //vuln
                    //pennib
                }
            }
        }

        public void StartTurn(Entity parent, EffectSet startTurnEf)
        {
            Status.StartTurn(parent, this, startTurnEf);
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

            //shoudl I just return here, to not apply the status?
            if (Duration == 0)
            {
                return;
                //throw new Exception("Expired status being applied.");
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
