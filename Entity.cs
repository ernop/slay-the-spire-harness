using System;
using System.Collections.Generic;
using System.Linq;

namespace StS
{
    public abstract class Entity : IEntity
    {
        public Entity(string name, EntityType entityType, int hpMax, int hp)
        {
            Name = name;
            EntityType = entityType;
            HPMax = hpMax;
            HP = hp;
        }

        public event NotifyTookDamage TakeDamage;

        public delegate void NotifyTookDamage(Entity e);

        public EntityType EntityType { get; set; }
        public string Name { get; set; }
        public int HP { get; set; }
        public int HPMax { get; set; }
        public int Block { get; set; }
        public bool Dead { get; set; }

        public List<StatusInstance> StatusInstances { get; set; } = new List<StatusInstance>();

        //unlike statuses and cards which have abstract Card and instances, relics are just relics
        public List<Relic> Relics { get; set; } = new List<Relic>();

        public bool CheckTurnip(StatusInstance si)
        {
            if (si.Status.StatusType == StatusType.Frail)
            {
                if (Relics.Any(el => el.Name == nameof(Turnip)))
                {
                    return true;
                }
            }
            return false;
        }

        public void ApplyStatus(Deck d, StatusInstance statusInstance)
        {
            if (CheckTurnip(statusInstance)) return;
            var exiStatus = StatusInstances.SingleOrDefault(el => el.Status.StatusType == statusInstance.Status.StatusType);
            if (exiStatus == null)
            {
                StatusInstances.Add(statusInstance);
                if (Helpers.PrintDetails)
                {
                    Console.WriteLine($"\tGained {statusInstance}");
                    Console.WriteLine(this);
                }
                statusInstance.Apply(d, this);
            }
            else
            {
                //combining statuses.  setting up the if this way avoids the ambiguity between flame barrier (scalable, impermanent) and pennib (unscalable, impermanent)
                if (statusInstance.Status.Scalable)
                {
                    exiStatus.Intensity += statusInstance.Intensity; //flame barrier, strength
                }
                else
                {
                    //vuln
                    exiStatus.Duration += statusInstance.Duration;
                }
                if (exiStatus.Duration == 0 || exiStatus.Intensity == 0)
                {
                    StatusInstances.Remove(exiStatus);
                    exiStatus.Unapply(d, this);
                }
                //we never apply the new status so it's inactive. we just mined it for intensity.
            }
        }

        /// <summary>
        /// after block is accounted.
        /// bool represents if they are still alive.
        /// </summary>
        public bool ApplyDamage(int amount)
        {
            HP -= amount;
            TakeDamage?.Invoke(this);

            return true;
        }

        public string Details()
        {
            var statuses = string.Join(",", StatusInstances.Select(el => el.ToString()));
            return $"{Name} HP: {HP}/{HPMax} Block:{Block} {statuses}";
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
