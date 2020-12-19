using System;
using System.Collections.Generic;

namespace StS
{
    /// <summary>
    /// It would be nice to evaluate these without side effects so that I could test them.
    /// </summary>
    public class Progression
    {
        public string Desc { get; set; }
        public Func<double, IEntity, double> Fun { get; set; }
        public int Order { get; set; }

        public Progression(string desc, Func<double, IEntity, double> fun, int order = 1)
        {
            Desc = desc;
            Fun = fun;
            Order = order;
        }

        public override string ToString()
        {
            return $"Prog:{Desc}";
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
