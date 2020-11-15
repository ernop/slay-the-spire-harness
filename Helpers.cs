using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        internal static Tuple<string,int> SplitCardName(string name)
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
                if (ael.Status.StatusType!=bel.Status.StatusType)
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

    }
}
