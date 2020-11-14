using System.Collections.Generic;

namespace StS
{
    public class Defend : Card
    {
        public override string Name => nameof(Defend);
        public override CharacterType CharacterType => CharacterType.IronClad;
        public override CardType CardType => CardType.Skill;

        public override ActionTarget ActionTarget => ActionTarget.Player;

        internal override EffectSet Apply(Player player, Enemy enemy, List<Enemy> enemyList, int upgradeCount)
        {
            int amount;
            if (upgradeCount == 0)
            {
                amount = 5;
            }
            else
            {
                amount = 6;
            }

            var ef = new EffectSet();
            ef.PlayerGainBlock.Add((_) => amount);
            return ef;
        }
    }
}
