using System.Collections.Generic;
using System.Linq;

namespace StS
{
    public class LimitBreak : Card
    {
        public override string Name => "Limit Break";

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override CardType CardType => CardType.Skill;

        public override void Apply(Player player, Enemy enemy, List<Enemy> enemyList, int upgradeCount)
        {
            var exi = player.Statuses.SingleOrDefault(el => el.Status.StatusType == StatusType.Strength);
            if (exi == null)
            {

            }
            else
            {
                //we could just double this in place but cleaner to reapply the same status
                var statusCopy = new StatusInstance(exi.Status, exi.Duration, exi.Intensity);
                player.ApplyStatus(statusCopy);
            }
        }
    }
}
