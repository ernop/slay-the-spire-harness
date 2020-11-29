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
        public int? PerTurnOverrideEnergyCost { get; set; } = null;
        public int? PerFightOverrideEnergyCost { get; set; } = null;
        public bool OverrideExhaust { get; set; }
        public CardInstance(Card card, int upgradeCount)
        {
            Card = card ?? throw new ArgumentNullException(nameof(card));
            UpgradeCount = upgradeCount;
        }

        public bool Exhausts()
        {
            if (OverrideExhaust)
            {
                return true;
            }
            return Card.Exhausts(UpgradeCount);
        }

        public bool Ethereal()
        {
            return Card.Ethereal(UpgradeCount);
        }

        /// <summary>
        /// Called when in hand at end of turn, OR when actively discarded.
        /// </summary>
        public void LeavingHand()
        {
            PerTurnOverrideEnergyCost = null;
        }

        public int EnergyCost()
        {
            if (PerTurnOverrideEnergyCost != null)
            {
                return PerTurnOverrideEnergyCost.Value;
            }
            if (PerFightOverrideEnergyCost != null)
            {
                return PerFightOverrideEnergyCost.Value;
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

        public void Play(EffectSet ef, IEntity source, IEntity target, List<CardInstance> cardTargets = null, Deck deck = null)
        {
            Card.Play(ef, source, target, UpgradeCount, cardTargets, deck);
            if (Helpers.PrintDetails)
            {
                Console.WriteLine($"\tplayed card {this}");
            }
        }

        /// <summary>
        /// full unique identifier of a per-fight or per-hand cardInstance
        /// </summary>
        public override string ToString()
        {
            var ec = EnergyCost();
            var upgrade = UpgradeCount > 0 ? "+" : "";
            var extra = "";
            if (PerTurnOverrideEnergyCost != null)
            {
                extra = $"(Turn:{ec})";
            }
            else if (PerFightOverrideEnergyCost != null)
            {
                extra = $"(Fight:{ec})";
            }
            return $"{Card}{upgrade}C:{ec}{extra}";
        }

        internal CardInstance Copy()
        {
            var ci = new CardInstance(Card, UpgradeCount);
            ci.PerTurnOverrideEnergyCost = PerTurnOverrideEnergyCost;
            ci.PerFightOverrideEnergyCost = PerFightOverrideEnergyCost;
            ci.OverrideExhaust = OverrideExhaust;
            return ci;
        }
    }
}
