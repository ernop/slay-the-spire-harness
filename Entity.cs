using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace StS
{
    public class Entity
    {

        public Entity(string name, GameContext gameContext, EntityType entityType, int hpMax, int hp)
        {
            Name = name;
            EntityType = entityType;
            HPMax = hpMax;
            HP = hp;
            GameContext = gameContext;
        }

        private GameContext GameContext { get; set; }
        public EntityType EntityType { get; set; }
        private string Name { get; set; }
        public int HP { get; set; }
        public int HPMax { get; set; }
        public int Block { get; set; }

        public List<StatusInstance> StatusInstances { get; set; } = new List<StatusInstance>();
        
        //unlike statuses and cards which have abstract Card and instances, relics are just relics
        public List<Relic> relics { get; set; } = new List<Relic>();

        /// <summary>
        /// Add duration (if not infinite).
        /// Seems that dexterity and strength are not really teh same as other statuses.
        /// </summary>
        /// <param name="statusInstance"></param>
        public void ApplyStatus(StatusInstance statusInstance)
        {
            var exiStatus = StatusInstances.SingleOrDefault(el => el.Status.StatusType == statusInstance.Status.StatusType);
            if (exiStatus == null)
            {
                StatusInstances.Add(statusInstance);
                if (Helpers.PrintDetails)
                {
                    Console.WriteLine($"\tGained {statusInstance}");
                }
            }
            else
            {
                if (exiStatus.Duration != int.MaxValue)
                {
                    exiStatus.Duration += statusInstance.Duration;
                }
                if (exiStatus.Intensity != int.MaxValue)
                {
                    exiStatus.Intensity += statusInstance.Intensity;
                }
                if (Helpers.PrintDetails)
                {
                    Console.WriteLine($"\tStatus changed to: {exiStatus}");
                }
                
                //remove pen nib when it's called, for example.
                //or remove strength when it reaches zero
                if (exiStatus.Duration == 0 || exiStatus.Intensity==0)
                {
                    StatusInstances.Remove(exiStatus);
                }

                //todo some statuses can have negative intensity but others can't.
            }
            //check for applying damage on status addition.
        }

        public void ApplyDamage(int amount)
        {
            if (amount >= HP)
            {
                _Die();
                if (Helpers.PrintDetails)
                {
                    Console.WriteLine($"\t{Name} Died of overdamage.");
                }
            }
            else
            {
                HP -= amount;
                if (Helpers.PrintDetails)
                {
                    Console.WriteLine($"\t{Name} took {amount} Damage");
                    if (amount == 0)
                    {
                        var ae = 3;
                    }
                }
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

        private void _Die()
        {
            GameContext.Died(this);
        }

        public override string ToString()
        {
            var statuses = string.Join(",", StatusInstances.Select(el => el.ToString()));
            return $"{EntityType} '{Name}' HP: {HP}/{HPMax} Block:{Block} {statuses}";
        }
    }
}
