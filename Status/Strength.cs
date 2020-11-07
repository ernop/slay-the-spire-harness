namespace StS
{
    public class Strength : Status
    {
        public override string Name => nameof(Strength);

        public override StatusType StatusType => StatusType.Strength;

        public override int AdjustDealtDamage(int amount, int duration, int intensity)
        {
            return amount + intensity;
        }
    }
}
