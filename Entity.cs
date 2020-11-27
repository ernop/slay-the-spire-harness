using System;
using System.Collections.Generic;
using System.Linq;

namespace StS
{
    public class Entity
    {

        public Entity(string name, EntityType entityType, int hpMax, int hp)
        {
            Name = name;
            EntityType = entityType;
            HPMax = hpMax;
            HP = hp;
        }

        public EntityType EntityType { get; set; }
        private string Name { get; set; }
        public int HP { get; set; }
        public int HPMax { get; set; }
        public int Block { get; set; }

        public List<StatusInstance> StatusInstances { get; set; } = new List<StatusInstance>();

        //unlike statuses and cards which have abstract Card and instances, relics are just relics
        public List<Relic> Relics { get; set; } = new List<Relic>();

        public void ApplyStatus(StatusInstance statusInstance)
        {
            var exiStatus = StatusInstances.SingleOrDefault(el => el.Status.StatusType == statusInstance.Status.StatusType);
            if (exiStatus == null)
            {
                StatusInstances.Add(statusInstance);
                if (Helpers.PrintDetails)
                {
                    Console.WriteLine($"\tGained {statusInstance}");
                    Console.WriteLine(this);
                }
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
                }
            }
        }

        public event Notify TakeDamage;

        public delegate void Notify(Entity e);

        /// <summary>
        /// after block is accounted.
        /// bool represents if they are still alive.
        /// </summary>
        public bool ApplyDamage(int amount)
        {
            if (amount >= HP)
            {

                if (Helpers.PrintDetails)
                {
                    Console.WriteLine($"\t{Name} Died of overdamage.");
                }
                return false;
            }
            else
            {
                HP -= amount;
                TakeDamage?.Invoke(this);
                if (Helpers.PrintDetails)
                {
                    Console.WriteLine($"\t{Name} took {amount} Damage");
                    if (amount == 0)
                    {
                        throw new Exception("Took zero damage?");
                    }
                }
                return true;
            }
        }

        public void ApplyBlock(int amount)
        {
            Block += amount;
            if (Helpers.PrintDetails)
            {
                Console.WriteLine($"\t{Name} Gained {amount} block.");
            }
        }

        public override string ToString()
        {
            var statuses = string.Join(",", StatusInstances.Select(el => el.ToString()));
            return $"{EntityType} '{Name}' HP: {HP}/{HPMax} Block:{Block} {statuses}";
        }
    }
}
