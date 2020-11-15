using System;

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
}
