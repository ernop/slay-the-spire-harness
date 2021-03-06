﻿using System;
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

        public Card Card { get; set; }
        public EnergyCostInt PerTurnOverrideEnergyCost { get; set; } = null;
        public EnergyCostInt PerFightOverrideEnergyCost { get; set; } = null;
        public bool OverrideExhaust { get; set; }
        public int Id { get; set; }
        public int UpgradeCount { get; private set; }

        public CardInstance(Card card, int upgradeCount)
        {
            Card = card ?? throw new ArgumentNullException(nameof(card));
            UpgradeCount = upgradeCount;
        }

        public bool Upgradeable()
        {
            if (Card.CardType == CardType.Status || Card.CardType == CardType.Curse)
            {
                //status & curse cards can't upgrade (although they can show up upgraded directly)
                return false;
            }
            if (Card.MultiUpgrade)
            {
                return true;
            }
            if (UpgradeCount == 0)
            {
                return true;
            }
            return false;
        }

        public void Upgrade()
        {
            if (!Upgradeable())
            {
                return;
            }
            if (Card.MultiUpgrade)
            {
                UpgradeCount++;
            }
            else if (UpgradeCount == 0)
            {
                UpgradeCount++;
            }
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
        public void LeavingHand(EffectSet ef)
        {
            PerTurnOverrideEnergyCost = null;
            //HandleLeavingHand(ef);
        }

        public EnergyCostInt EnergyCost()
        {
            if (PerTurnOverrideEnergyCost != null)
            {
                return PerTurnOverrideEnergyCost;
            }
            if (PerFightOverrideEnergyCost != null)
            {
                return PerFightOverrideEnergyCost;
            }
            return Card.CiCanCallEnergyCost(UpgradeCount);
        }

        /// <summary>
        /// player should be able to query a card to see if it's currently playable.
        /// </summary>
        public bool Playable(IList<CardInstance> hand)
        {
            if (!Card.Playable(hand))
            {
                return false;
            }
            return true;
        }

        public virtual void Play(EffectSet ef, Player player, IEnemy enemy, IList<CardInstance> cardTargets = null, Deck deck = null, long? key = null)
        {
            Card.Play(ef, player, enemy, UpgradeCount, cardTargets, deck, key);
        }

        public void OtherAction(Action action, EffectSet ef)
        {
            Card.OtherAction(action, ef, UpgradeCount);
        }

        internal void StartFight()
        {
            PerTurnOverrideEnergyCost = null;
            PerFightOverrideEnergyCost = null;
        }

        internal void LeftInHandAtEndOfTurn(IndividualEffect playerEffect)
        {
            Card.LeftInHandAtEndOfTurn(playerEffect, UpgradeCount);
        }

        /// <summary>
        /// full unique identifier of a per-fight or per-hand cardInstance
        /// </summary>
        public override string ToString()
        {
            var ec = EnergyCost();
            var upgrade = UpgradeCount > 0 ? "+" : "";
            if (UpgradeCount > 1)
            {
                upgrade = $"+{UpgradeCount}";
            }

            var extra = "";
            if (PerTurnOverrideEnergyCost != null)
            {
                extra = $"(Turn:{ec})";
            }
            else if (PerFightOverrideEnergyCost != null)
            {
                extra = $"(Fight:{ec})";
            }

            return $"{Card}{upgrade}{extra}";
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
