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
        public Func<int, int> Fun { get; set; }

        public Progression(string desc, Func<int, int> fun)
        {
            Desc = desc;
            Fun = fun;
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
        public Func<List<int>, List<int>> Fun { get; set; }

        public AttackProgression(string desc, Func<List<int>,List<int>> func)
        {
            Desc = desc;
            Fun = func;
        }

        public override string ToString()
        {
            return $"Prog:{Desc}";
        }
    }
}
