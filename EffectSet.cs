﻿using System;
using System.Collections.Generic;

namespace StS
{
    /// <summary>
    /// when a card is played on an entity, which extra effectset does it generate? (on player or enemy?) due to their statuses and relics?
    /// Example: pen nib returns a damage doubler
    /// 
    /// Current assumption: only one enemy
    /// 
    /// Real StS separates out extra damage from enemy multiattacks which this would all combine.
    /// PlayerReceivesDamage is combining implementation with the product request
    /// </summary>
    public class EffectSet
    {
        public IndividualEffect SourceEffect = new IndividualEffect();
        public IndividualEffect TargetEffect = new IndividualEffect();
        public int PlayerEnergy { get; set; } = 0;

        /// <summary>
        /// After playing the card, call these on the hand (temporarily); things like monkey paw.
        /// </summary>
        public List<Action<Deck>> DeckEffect { get; set; } = new List<Action<Deck>>();
        public List<Action<Player>> PlayerEffect { get; set; } = new List<Action<Player>>();

        /// <summary>
        /// The ultimate power; can see everything.
        /// </summary>
        public List<OneEffect> FightEffects { get; set; } = new List<OneEffect>();
    }

    public class OneEffect
    {
        public Action<Fight, Deck> Action { get; set; }
    }


    public class IndividualEffect
    {
        public double InitialBlock { get; set; }
        public IEnumerable<int> InitialDamage { get; set; }
        public List<Progression> BlockAdjustments { get; set; } = new List<Progression>();
        public List<AttackProgression> DamageAdjustments { get; set; } = new List<AttackProgression>();
        public List<StatusInstance> Status { get; set; } = new List<StatusInstance>();
    }
}
