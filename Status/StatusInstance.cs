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
        /// Should be autodecremented upon round advancement, unless infinite.
        /// </summary>
        public int Duration { get; set; }
        
        /// <summary>
        /// The "strength" of the effect. If amount==int.maxint it's just "on" like you are vuln or not vuln, no scale.
        /// strength 2 is a 2 intensity statusInstance
        /// </summary>
        public int Intensity { get; set; }
        
        /// <summary>
        /// quite bad that intensity needs to be hardcoded when for some statuses it's actually just fixed.
        /// </summary>
        public StatusInstance(Status status, int duration, int intensity)
        {
            Status = status;
            Duration = duration;
            Intensity = intensity;
        }

        public void NewTurnStarted()
        {
            if (Duration != int.MaxValue)
            {
                Duration--;
            }
            if (Duration == 0)
            {
                //remove the status.
            }
        }
        
        public override string ToString()
        {
            var dur = Duration == int.MaxValue ? "" : " :"+Duration.ToString();
            var amt = Intensity == int.MaxValue ? "" : " :" + Intensity.ToString();
            return $"{Status.Name}{dur}{amt}";
        }

        public void Apply(Card card, EffectSet ef)
        {
            Status.Apply(card, ef, Intensity);
        }
    }
}
