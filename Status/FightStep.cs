using System;
using System.Collections.Generic;

namespace StS
{
    /// <summary>
    /// It would be nice to evaluate these without side effects so that I could test them, and also display results before they happen.
    /// </summary>
    public class FightStep
    {
        public string Desc { get; set; }
        /// <summary>
        /// For a given planned block gain (double) on entity, how much should we actually gain?
        /// </summary>
        public double Amount { get; set; }
        public int Order { get; set; }
        public bool Additive { get; } = true;

        public FightStep(string desc, double amt, int? order = 1, bool additive = true)
        {
            var usingOrder = order ?? 1;
            
            Desc = desc;
            Amount = amt;
            Order = usingOrder;
            Additive = additive;
        }

        public override string ToString()
        {
            var ad = Additive ? "+" : "*";
            return $"{Desc}:{ad}{Amount}";
        }
    }

    /// <summary>
    /// Allowing for multiattack.
    /// </summary>
    public class AttackProgression
    {
        public string Desc { get; set; }
        public Func<IList<double>, IList<double>> Fun { get; set; }

        /// <summary>
        /// This should be used to take over InitialDamage and only have damageadjustments.
        /// </summary>
        public int Order { get; set; }

        public AttackProgression(string desc, Func<IList<double>, IList<double>> func, int order = 1)
        {
            Desc = desc;
            Fun = func;
            Order = order;
        }

        public override string ToString()
        {
            return $"Prog:{Desc}";
        }
    }
}
