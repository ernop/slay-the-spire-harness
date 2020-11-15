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
        public Func<int,Entity,int> Fun { get; set; }

        public Progression(string desc, Func<int,Entity, int> fun)
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
        public Func<IEnumerable<int>, IEnumerable<int>> Fun { get; set; }

        public AttackProgression(string desc, Func<IEnumerable<int>, IEnumerable<int>> func)
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
