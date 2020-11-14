namespace StS
{
    public class Varja : Relic
    {
        public int Intensity { get; set; } = 0;
        public override void CardPlayed(Card card, EffectSet ef)
        {
            if (Intensity > 0 && card.CardType==CardType.Attack)
            {
                ef.EnemyReceivesDamage.Add((el) =>
                {
                    if (el > 0)
                    {
                        return el + Intensity;
                    }
                    return 0;
                });
            }
            
        }

        public override void FightStarted()
        {
            return;
        }

        public override void NewTurn()
        {
            return;
        }

        public override string ToString()
        {
            return $"Varja:{Intensity}";
        }
    }
}
