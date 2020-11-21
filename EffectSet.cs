using System;
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
        public int PlayerMana { get; set; } = 0;
        public HandEffect HandEffect { get; set; }
    }

    public enum HandEffect
    {
        PullACardFromDiscardToTopOfDraw = 1,
        CannotPlay = 2, //clash
    }

    public class IndividualEffect
    {
        public int InitialBlock { get; set; }
        public IEnumerable<int> InitialDamage { get; set; }
        public List<Progression> BlockAdjustments { get; set; } = new List<Progression>();
        public List<AttackProgression> DamageAdjustments { get; set; } = new List<AttackProgression> ();
        public List<StatusInstance> Status { get; set; } = new List<StatusInstance>();
    }
}
