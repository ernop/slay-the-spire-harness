using System;
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
        public abstract TargetType TargetType { get; }
        internal abstract void Apply(EffectSet ef, Entity source, Entity target, int upgradeCount);
        public abstract bool Exhausts(int upgradeCount);
        public abstract bool Ethereal(int upgradeCount);
        public abstract int EnergyCost(int upgradeCount);

        public override string ToString()
        {
            return Name;
        }


    }

    public abstract class SkillCard : Card
    {
        public override CardType CardType => CardType.Skill;
    }

    public abstract class PowerCard : Card
    {
        public override TargetType TargetType => TargetType.Player;
        public override CardType CardType => CardType.Power;
    }
    public abstract class CurseCard : Card
    {
        public override TargetType TargetType => TargetType.Player;
        public override CardType CardType => CardType.Curse;
    }
    public abstract class AttackCard : Card
    {
        public override TargetType TargetType => TargetType.Enemy;
        public override CardType CardType => CardType.Attack;
    }

    public abstract class EnemyCard : Card
    {
        public override CardType CardType => CardType.Attack;
    }
}
