using System.Collections.Generic;

namespace StS
{
    public class Sundial : Relic
    {
        public override string Name => nameof(Sundial);
        public int ShuffleCount { get; set; }

        internal override Relic Copy()
        {
            return new Sundial
            {
                ShuffleCount = ShuffleCount
            };
        }

        public override void StartFight(Deck deck, EffectSet ef)
        {
            deck.DeckShuffle += DeckShuffle;
        }

        public override void EndFight(Deck deck, EffectSet ef)
        {
            deck.DeckShuffle -= DeckShuffle;
        }

        private void DeckShuffle(EffectSet ef, List<string> history)
        {
            ShuffleCount += 1;
            if (ShuffleCount % 3 == 0)
            {
                history.Add("Gained 2 energy from Sundial shuffle");
                ef.PlayerEnergy = 2;
            }
        }

        public override string ToString()
        {
            return $"{nameof(Sundial)}:{ShuffleCount}";
        }
    }
}
