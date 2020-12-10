using System;
using System.Collections.Generic;
using System.Linq;

namespace StS
{
    public class Player : Entity, IEntity
    {
        public Player(CharacterType type = CharacterType.IronClad, int? hpMax = null, int? hp = null, IEnumerable<Relic> relics = null, IEnumerable<Potion> potions = null) : base("Wilson", EntityType.Player, hpMax ?? 100, hp ?? 100
            )
        {
            CharacterType = type;
            if (relics != null)
            {
                foreach (var relic in relics)
                {
                    relic.Player = this;
                    Relics.Add(relic);
                }
            }
            if (potions != null)
            {
                //TODO check max potion slots?
                Potions = potions.ToList();
            }
        }

        internal Player Copy()
        {
            //var relics = Relics.Select(el => el.Copy());
            var newPlayer = new Player(CharacterType);
            //newPlayer.Block = Block;
            newPlayer.Energy = Energy;
            newPlayer.Gold = Gold;
            newPlayer.Potions = Potions.Select(el => el.Copy()).ToList();
            //TODO not copying potions now.
            CopyEntity(newPlayer);
            return newPlayer;
        }

        public CharacterType CharacterType { get; }
        public int Energy { get; set; }
        public int Gold { get; private set; }
        public void GainGold(int amount)
        {
            if (Relics.SingleOrDefault(el => el.Name == "Ectoplasm") != null)
            {
                return;
            }
            Gold += amount;
        }

        public void DrinkPotion(Fight f, Potion p, Enemy e)
        {
            var ef = new EffectSet();
            p.Apply(f, this, e, ef);
            Entity target;
            if (p.SelfTarget())
            {
                target = this;
            }
            else
            {
                target = e;
            }
            f.ApplyEffectSet(FightActionEnum.Potion, ef, this, target, potion: p);
        }

        public List<Potion> Potions { get; set; } = new List<Potion>();

        public int MaxEnergy()
        {
            var value = 3;
            foreach (var relic in Relics)
            {
                if (relic.ExtraEnergy)
                {
                    value++;
                }
            }
            return value;
        }

        internal int GetDrawAmount()
        {
            ///todo this should be affected by statuses.
            return 5;
        }

        internal void HealFor(int amount, out string healRes)
        {
            var before = HP;
            HP = Math.Min(HPMax, HP + amount);
            var healAmt = HP - before;
            healRes = $"Healed for {healAmt}";
        }
    }
}
