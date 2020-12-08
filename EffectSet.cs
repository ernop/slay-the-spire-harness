using System;
using System.Collections.Generic;
using System.Linq;

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
        public List<Func<Deck, string>> DeckEffect { get; set; } = new List<Func<Deck, string>>();
        public List<Func<Player, string>> PlayerEffect { get; set; } = new List<Func<Player, string>>();

        /// <summary>
        /// The ultimate power; can see everything.
        /// </summary>
        public List<OneEffect> FightEffect { get; set; } = new List<OneEffect>();

        public override string ToString()
        {
            var se = SourceEffect.ToString();
            var te = TargetEffect.ToString();
            var pen = PlayerEnergy == 0 ? "" : $"+({PlayerEnergy})";
            var de = DeckEffect.Count == 0 ? "" : $"DeckEffect:{DeckEffect.Count}";
            var ple = PlayerEffect.Count == 0 ? "" : $"PlayerEffect:{PlayerEffect.Count}";
            var fe = FightEffect.Count == 0 ? "" : $"FightEffects:{FightEffect.Count}";

            var res = string.Join('_', new List<string>() { se, te, pen, de, ple, fe }.Where(el => !string.IsNullOrEmpty(el)));

            return res;
        }

    }

    public class OneEffect
    {
        public Func<Fight, Deck, string> Action { get; set; }
    }


    public class IndividualEffect
    {
        public double InitialBlock { get; set; }
        public IList<int> InitialDamage { get; set; }
        public List<Progression> BlockAdjustments { get; set; } = new List<Progression>();
        public List<AttackProgression> DamageAdjustments { get; set; } = new List<AttackProgression>();
        public List<StatusInstance> Status { get; set; } = new List<StatusInstance>();

        public override string ToString()
        {

            var ib = InitialBlock == 0 ? "" : $"B:{InitialBlock}";
            var id = InitialDamage.Count == 0 ? "" : $"D:{string.Join(',', InitialDamage)}";
            var ba = string.Join(',', BlockAdjustments.Select(el => el.Desc));
            var da = string.Join(',', DamageAdjustments.Select(el => el.Desc));
            var s = string.Join(',', Status.Select(el => el.ToString()));
            var res = string.Join('|', new List<string>() { ib, id, ba, da, s }.Where(el => !string.IsNullOrEmpty(el)));

            return res;
        }

    }
}
