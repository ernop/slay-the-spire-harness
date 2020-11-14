using System.Collections.Generic;

namespace StS
{
    public class Footwork : Card
    {
        public override string Name => nameof(Footwork);

        public override CharacterType CharacterType => CharacterType.Silent;

        public override CardType CardType => CardType.Power;

        internal override EffectSet Apply(Player player, Enemy enemy, List<Enemy> enemyList, int upgradeCount)
        {
            var ef = new EffectSet();
            int amt;
            if (upgradeCount == 0)
            {
                amt = 2;
            }
            else
            {
                amt = 3;
            }

            ef.PlayerStatus.Add(new StatusInstance(new Dexterity(), int.MaxValue, amt));
            return ef;
        }
    }
}
