using System.Collections.Generic;

namespace StS
{
    public class IronWave : Card
    {
        public override string Name => nameof(IronWave);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override CardType CardType => CardType.Attack;

        internal override EffectSet Apply(Player player, Enemy enemy, List<Enemy> enemyList, int upgradeCount)
        {
            int dmg;
            int block;
            if (upgradeCount == 0)
            {
                dmg = 5;
                block = 5;
            }
            else
            {
                dmg = 7;
                block = 7;
            }

            var ef = new EffectSet();
            ef.EnemyReceivesDamage.Add((_) => dmg);
            ef.PlayerGainBlock.Add((_) => block);
            return ef;
        }
    }
}
