using System;
using System.Collections.Generic;

namespace StS
{
    /// <summary>
    /// when a card is played on an entity, which extra effectset does it generate? (on player or enemy?) due to their statuses and relics?
    /// Example: pen nib returns a damage doubler
    /// 
    /// Current assumption: only one enemy
    /// </summary>
    public class EffectSet
    {
        public List<Func<int, int>> PlayerGainBlock { get; set; } = new List<Func<int, int>>();
        public List<Func<int, int>> PlayerReceivesDamage { get; set; } = new List<Func<int, int>>();
        public List<StatusInstance> PlayerStatus { get; set; } = new List<StatusInstance>();
        public List<Func<int, int>> EnemyGainsBlock { get; set; } = new List<Func<int, int>>();
        public List<Func<int, int>> EnemyReceivesDamage { get; set; } = new List<Func<int, int>>();
        public List<StatusInstance> EnemyStatus { get; set; } = new List<StatusInstance>();
    }
}
