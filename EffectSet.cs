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
    }

    public class IndividualEffect
    {
        public List<Progression> GainBlock { get; set; } = new List<Progression>();
        public List<Progression> ReceiveDamage { get; set; } = new List<Progression> ();
        public List<StatusInstance> Status { get; set; } = new List<StatusInstance>();
    }
}
