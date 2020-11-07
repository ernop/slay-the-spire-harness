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
            var exi = entity.Statuses.SingleOrDefault(el => el.Status.StatusType == StatusType.Vulnerable);
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
    }
}
