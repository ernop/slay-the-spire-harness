using System;
using System.Collections.Generic;
using System.Linq;

namespace StS
{
    public class Player : Entity
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
            var relics = Relics.Select(el => el.Copy());
            var newPlayer = new Player(CharacterType, HPMax, HP, relics);
            newPlayer.Energy = Energy;
            newPlayer.Gold = Gold;
            newPlayer.Potions = Potions.Select(el => el.Copy()).ToList();
            //TODO not copying potions now.
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

        public void DrinkPotion(Potion p, Enemy e)
        {
            p.Apply(this, e);
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

        internal void HealFor(int amount)
        {
            HP = Math.Min(HPMax, HP + amount);
        }
    }
}
