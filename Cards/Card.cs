using System.Collections.Generic;
using System.Text;

using static StS.Helpers;

namespace StS
{
    public abstract class Card
    {
        public abstract string Name { get;}
        public string Text { get;}
        public abstract CharacterType CharacterType { get; }
        public abstract CardType CardType { get; }
        public abstract void Apply(Player player, Enemy enemy, List<Enemy> enemyList, int upgradeCount);
        public override string ToString()
        {
            return Name;
        }
    }
}
