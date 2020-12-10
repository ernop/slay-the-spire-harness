using System.Collections.Generic;

namespace StS
{
    public interface IEntity
    {
        public EntityType EntityType { get; set; }
        public string Name { get; set; }
        public int HP { get; set; }
        public int HPMax { get; set; }
        public int Block { get; set; }
        public List<StatusInstance> StatusInstances { get; set; }
        public List<Relic> Relics { get; set; }
        public bool Dead { get; set; }
        public bool ApplyDamage(int amount);
        //public void ApplyBlock(int amount);
        public void ApplyStatus(Deck d, StatusInstance statusInstance);
        public string Details();
        public IEntity CopyEntity(IEntity entity);
    }
}
