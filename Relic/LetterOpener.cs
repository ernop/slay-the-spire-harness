﻿using System.Collections.Generic;

namespace StS
{
    public class LetterOpener : Relic
    {
        public override string Name => nameof(LetterOpener);
        private int SkillCt { get; set; } = 0;
        public override void StartTurn(Player player, IEntity enemy, EffectSet ef)
        {
            SkillCt = 0;
        }

        public override void CardPlayed(Card card, EffectSet ef, IEntity player, IEntity enemy)
        {
            if (card.CardType == CardType.Skill)
            {
                SkillCt++;
            }
            if (SkillCt % 3 == 0)
            {
                ef.EnemyEffect.InitialDamage = new List<int>() { 5 };
            }
        }

        internal override Relic Copy()
        {
            return new LetterOpener { SkillCt = SkillCt };
        }
    }
}