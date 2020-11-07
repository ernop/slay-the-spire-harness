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

        public List<StatusInstance> Statuses { get; set; } = new List<StatusInstance>();

        /// <summary>
        /// Add duration (if not infinite).
        /// Seems that dexterity and strength are not really teh same as other statuses.
        /// </summary>
        /// <param name="statusInstance"></param>
        public void ApplyStatus(StatusInstance statusInstance)
        {
            var exiStatus = Statuses.SingleOrDefault(el => el.Status.StatusType == statusInstance.Status.StatusType);
            if (exiStatus == null)
            {
                Statuses.Add(statusInstance);
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
            }
            //check for applying damage on status addition.
        }

        public int AdjustDealtDamage(int amount)
        {
            foreach (var si in Statuses)
            {
                amount = si.AdjustDealtDamage(amount);
            }
            return amount;
        }

        private int _AdjustReceivedDamage(int amount)
        {
            foreach (var si in Statuses)
            {
                amount = si.AdjustReceivedDamage(amount);
            }
            return amount;
        }

        public void ApplyDamage(int amount)
        {
            amount = _AdjustReceivedDamage(amount);
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
            var statuses = string.Join(",", Statuses.Select(el => el.ToString()));
            return $"{EntityType} '{Name}' HP: {HP}/{HPMax} Block:{Block} {statuses}";
        }
    }
}
