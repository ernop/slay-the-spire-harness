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

        //public void NewTurnStarted()
        //{
        //    if (!Status.Permanent)
        //    {
        //        Duration--;
        //    }
        //    if (Duration == 0)
        //    {
        //        Parent.StatusInstances.Remove(this);
        //    }
        //    if (Duration < 0)
        //    {
        //        throw new Exception("Negative Duration");
        //    }
        //}
        
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
