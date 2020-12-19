using System;
using System.Collections.Generic;
using System.Linq;

namespace StS
{
    public class Player : Entity, IEntity
    {
        public Player(CardDomain type = CardDomain.IronClad, int? hpMax = null, int? hp = null,
            IEnumerable<Relic> relics = null, IEnumerable<Potion> potions = null,
            int? maxEnergy = null, int? drawAmount = null)
            : base("Wilson", EntityType.Player, hpMax ?? 100, hp ?? 100)
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
            _MaxEnergy = maxEnergy ?? 3;
            _DrawAmount = drawAmount ?? 5;
        }

        internal Player Copy()
        {
            //var relics = Relics.Select(el => el.Copy());
            var newPlayer = new Player(CharacterType);
            //newPlayer.Block = Block;
            newPlayer.Energy = Energy;
            newPlayer.Gold = Gold;
            newPlayer.Potions = Potions.Select(el => el.Copy()).ToList();
            newPlayer._MaxEnergy = _MaxEnergy;
            newPlayer._DrawAmount = _DrawAmount;

            //TODO not copying potions now.
            CopyEntity(newPlayer);
            return newPlayer;
        }


        public CardDomain CharacterType { get; }

        public int Energy { get; set; }
        public int Gold { get; private set; }
        private int _DrawAmount { get; set; }
        private int _MaxEnergy { get; set; }

        public void GainGold(int amount)
        {
            if (Relics.SingleOrDefault(el => el.Name == "Ectoplasm") != null)
            {
                return;
            }
            Gold += amount;
        }



        public List<Potion> Potions { get; set; } = new List<Potion>();

        public int MaxEnergy()
        {
            var value = _MaxEnergy;
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
            ///TODO this should be affected by statuses.
            return _DrawAmount;
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
