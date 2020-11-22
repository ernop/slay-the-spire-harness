using System;
using System.Collections.Generic;

namespace StS
{

    /// <summary>
    /// per-fight or per-hand cardinstance.
    /// per-fight would include extra copies of cards like anger, for example.
    /// per-hand would have instances that were more upgraded (armaments) or had different costs (monkey paw).
    /// </summary>
    public class CardInstance
    {
        public int UpgradeCount { get; set; }
        public Card Card { get; set; }
        public int? OverrideEnergyCost { get; set; } = null;
        public CardInstance(Card card, int upgradeCount)
        {
            Card = card;
            UpgradeCount = upgradeCount;
        }

        /// <summary>
        /// Called when discarded
        /// </summary>
        public void EnteringDiscardPile()
        {
            OverrideEnergyCost = null;
        }

        public int EnergyCost()
        {
            if (OverrideEnergyCost != null)
            {
                return OverrideEnergyCost.Value;
            }
            return Card.CiCanCallEnergyCost(UpgradeCount);
        }

        /// <summary>
        /// player should be able to query a card to see if it's currently playable.
        /// </summary>
        public bool Playable(List<CardInstance> hand)
        {
            if (!Card.Playable(hand))
            {
                return false;
            }
            return true;
        }

        public void Play(EffectSet ef, Entity source, Entity target, List<CardInstance> cardTargets = null)
        {
            if (Helpers.PrintDetails)
            {
                Console.WriteLine($"\tplaying card {this}");
            }

            Card.Play(ef, source, target, UpgradeCount, cardTargets);
        }

        /// <summary>
        /// full unique identifier of a per-fight or per-hand cardInstance
        /// </summary>
        public override string ToString()
        {
            var ec = EnergyCost();
            var upgrade = UpgradeCount > 0 ? "+" : "";
            return $"{Card}{upgrade}C:{ec}";
        }
    }
}
