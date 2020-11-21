using System;
using System.Collections.Generic;
using System.Linq;

namespace StS
{
    public static class Helpers
    {
        public static bool PrintDetails = false;

        public static bool IsVulnerable(Entity entity)
        {
            var exi = entity.StatusInstances.SingleOrDefault(el => el.Status.StatusType == StatusType.Vulnerable);
            if (exi != null)
            {
                return true;
            }
            return false;
        }

        internal static Tuple<string, int> SplitCardName(string name)
        {
            //for now just detect trailing+
            var upgradeCount = 0;
            if (name.EndsWith("+"))
            {
                upgradeCount = 1;
                name = name.TrimEnd('+');
            }
            return new Tuple<string, int>(name, upgradeCount);
        }

        public static bool CompareStatuses(List<StatusInstance> a, List<StatusInstance> b, out string error)
        {
            if (a.Count != b.Count)
            {
                error = "Status length mismatch";
                return false;
            }
            for (var i = 0; i < a.Count; i++)
            {
                var ael = a[i];
                var bel = b[i];
                if (ael.Status.StatusType != bel.Status.StatusType)
                {
                    error = "Mismatching status";
                    return false;
                }
                if (ael.Intensity != bel.Intensity)
                {
                    error = "Mismatch intensity";
                    return false;
                }
                if (ael.Duration != bel.Duration)
                {
                    error = "Mismatch duration";
                    return false;
                }
            }
            error = "Okay";
            return true;
        }

        public static List<int> Repeat(int num, int count)
        {
            var ii = 0;
            var res = new List<int>();
            while (ii < count)
            {
                res.Add(num);
                ii++;
            }
            return res;
        }

        /// <summary>
        /// sometimes we'll have target effects (gaining block from playing "defend")
        /// and source effects (afterimage is a status that gives +1 block per card played)
        /// When target == source, we have to combine them.
        /// </summary>
        public static IndividualEffect Combine(IndividualEffect ef1, IndividualEffect ef2)
        {
            var combined = new IndividualEffect();
            combined.InitialBlock = ef1.InitialBlock + ef2.InitialBlock;
            var res = new List<int>();
            if (ef1.InitialDamage != null || ef2.InitialDamage != null)
            {
                foreach (var ls in new List<IEnumerable<int>>() { ef1.InitialDamage, ef2.InitialDamage })
                {
                    foreach (var el in ls)
                    {
                        res.Add(el);
                    }
                }
                combined.InitialDamage = res;
            }

            combined.DamageAdjustments.AddRange(ef1.DamageAdjustments);
            combined.DamageAdjustments.AddRange(ef2.DamageAdjustments);
            combined.BlockAdjustments.AddRange(ef1.BlockAdjustments);
            combined.BlockAdjustments.AddRange(ef2.BlockAdjustments);
            combined.Status.AddRange(ef1.Status);
            combined.Status.AddRange(ef2.Status);
            return combined;
        }

        /// <summary>
        /// meant to be a complete comparison of per-hand lists of CIs.
        /// </summary>
        public static bool CompareHands(List<CardInstance> a, List<CardInstance> b, out string message)
        {
            if (a.Count() != b.Count())
            {
                message = "length mismatch";
                return false;
            }
            var al = string.Join(',', a.Select(el => el.ToString()).OrderBy(el => el));
            var bl = string.Join(',', b.Select(el => el.ToString()).OrderBy(el => el));
            if (al != bl)
            {
                message = $"{al} != {bl}";
                return false;
            }

            message = "okay";
            return true;
        }

        public static CardInstance CopyCI(CardInstance ci)
        {
            var newCi = new CardInstance(ci.Card, ci.UpgradeCount);
            return newCi;
        }

        /// <summary>
        /// Null or a CI that has nonzero cost
        /// </summary>
        public static CardInstance SelectNonZeroCostCard(List<CardInstance> cis)
        {
            var rnd = new Random();
            var nonZeros = cis.Where(el => el.EnergyCost() > 0);
            var len = nonZeros.Count();
            if (len == 0)
            {
                return null;
            }
            var num = rnd.Next(len);
            return nonZeros.Skip(num).First();
        }
    }
}
