﻿using System.Collections.Generic;
using System.Linq;

namespace StS
{
    public class Headbutt : IroncladAttackCard
    {
        public override string Name => nameof(Headbutt);

        public override int CiCanCallEnergyCost(int upgradeCount) => 1;

        /// <summary>
        /// There needs to be a way to specify extra parameters to play, for things like:
        /// TrueGrit, etc.
        /// </summary>
        internal override void Play(EffectSet ef, IEntity source, IEntity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            int dmg;
            if (upgradeCount == 0)
            {
                dmg = 9;
            }
            else
            {
                dmg = 12;
            }
            ef.TargetEffect.InitialDamage = new List<int>() { dmg };

            ef.DeckEffect.Add((Deck deck) =>
            {
                var headbuttTarget = targets.First();
                if (!deck.DiscardPileContains(headbuttTarget))
                {
                    throw new System.Exception("Trying to headbutt card not in discard.");
                }

                deck.MoveFromDiscardToDraw(headbuttTarget);
            });
        }
    }
}
