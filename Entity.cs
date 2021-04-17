using System.Collections.Generic;
using System.Linq;

using static StS.Helpers;

namespace StS
{
    public abstract class Entity : IEntity
    {
        public Entity(string name, EntityType entityType, int? hp = null, int? hpMax = null)
        {
            Name = name;
            EntityType = entityType;

            if (hp != null && hpMax == null)
            {
                hpMax = hp;
            }

            HPMax = hpMax ?? 50;
            HP = hp ?? 50;
            if (HP > HPMax)
            {
                throw new System.Exception("Invalid HP>HPMax");
            }
        }

        public event NotifyBeAttacked BeAttacked;

        public delegate void NotifyBeAttacked(EffectSet ef);

        public event TakeDamageResponse TakeDamage;

        public delegate void TakeDamageResponse(EffectSet ef, int damageAmount, CardInstance ci);
        public void NotifyAttacked(EffectSet ef)
        {
            BeAttacked?.Invoke(ef);
        }

        public EntityType EntityType { get; set; }
        public string Name { get; set; }
        public int HP { get; set; }
        public int HPMax { get; set; }
        public int Block { get; set; }
        public bool Dead { get; set; } = false;

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

        public void ApplyStatus(Fight f, Deck d, StatusInstance statusInstance)
        {
            if (CheckTurnip(statusInstance)) return;
            var exiStatus = StatusInstances.SingleOrDefault(el => el.Status.StatusType == statusInstance.Status.StatusType);
            if (exiStatus == null)
            {
                if (statusInstance.Intensity < 0 && !statusInstance.Status.CanAddNegative)
                {
                    return;
                }

                StatusInstances.Add(statusInstance);
                statusInstance.Apply(f, d, this);
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
                    exiStatus.Unapply(f, d, this);
                }
                //we never apply the new status so it's inactive. we just mined it for intensity.
            }
        }

        /// <summary>
        /// after block is accounted.
        /// bool represents if they are still alive.
        /// For use by essence of steel status too.
        /// </summary>
        public bool ApplyDamage(int amount, EffectSet ef, CardInstance ci, List<string> history)
        {
            HP -= amount;
            //this is for any damage type.
            TakeDamage?.Invoke(ef, amount, ci);
            history.Add($"{Name} took {amount} dmg");

            return true;
        }

        public string Details()
        {
            var statuses = SJ('\t', StatusInstances.Select(el => el.ToString()));
            if (statuses.Length > 0)
            {
                statuses = " " + statuses;
            }
            var relics = " " + SJ('\t', Relics);
            return $"{Name} {HP}/{HPMax} B{Block}{statuses}{relics}";
        }

        public override string ToString()
        {
            return $"{Name}";
        }

        public IEntity CopyEntity(IEntity entity)
        {
            var sis = StatusInstances.Select(el => el.Copy()).ToList();

            entity.Name = Name;
            entity.Dead = Dead;
            entity.Block = Block;
            entity.EntityType = EntityType;
            entity.Relics = Relics.Select(el => el.Copy()).ToList();
            entity.StatusInstances = sis;
            entity.HP = HP;
            entity.HPMax = HPMax;
            return entity;
        }
    }
}
