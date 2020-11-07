namespace StS
{
    public class StatusInstance
    {
        /// <summary>
        /// Should be autodecremented upon round advancement, unless infinite.
        /// </summary>
        public int Duration { get; set; }
        
        /// <summary>
        /// The "strength" of the effect. If amount==1 it's just "on" like you are vuln or not vuln, no scale.
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

        public Status Status { get; set; }
        public override string ToString()
        {
            var dur = Duration == int.MaxValue ? "" : " :"+Duration.ToString();
            var amt = Intensity == int.MaxValue ? "" : " :" + Intensity.ToString();
            return $"{Status.Name}{dur}{amt}";
        }

        public int GainsBlock(int amount)
        {
            return Status.GainsBlock(amount);
        }

        /// <summary>
        /// When someone with a status does damage, all statusInstances will be consulted to see if they affect it.
        /// They call into their definitions,  with their instance params so we know how much to scale (example: strength)
        /// </summary>
        public int AdjustDealtDamage(int amount)
        {
            return Status.AdjustDealtDamage(amount, Duration, Intensity);
        }

        public int AdjustReceivedDamage(int amount)
        {
            return Status.AdjustReceivedDamage(amount, Duration, Intensity);
        }
    }
}
