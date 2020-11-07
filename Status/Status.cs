namespace StS
{
    public abstract class Status
    {
        public abstract string Name { get; }
        public abstract StatusType StatusType { get; }
        
        /// <summary>
        /// Does this affect damage received
        /// </summary>
        public int ReceiveDamage(int amount)
        {
            return amount;
        }

        /// <summary>
        /// Does this affect damage dealt. This is the fallthrough.
        /// </summary>
        public virtual int AdjustDealtDamage(int amount, int duration, int intensity)
        {
            return amount;
        }

        public virtual int AdjustReceivedDamage(int amount, int duration, int intensity)
        {
            return amount;
        }
        public virtual int GainsBlock(int amount)
        {
            return amount;
        }
    }
}
