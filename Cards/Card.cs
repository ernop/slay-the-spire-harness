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
        internal abstract void Play(EffectSet ef, Entity source, Entity target, int upgradeCount);
        public abstract bool Exhausts(int upgradeCount);
        public abstract bool Ethereal(int upgradeCount);
        public abstract int EnergyCost(int upgradeCount);
        
        /// <summary>
        /// player should be able to query a card to see if it's currently playable.
        /// </summary>
        public bool Playable(int energy, int upgradeCount)
        {
            return EnergyCost(upgradeCount) <= energy;
        }

        /// <summary>
        /// Does the card need to be called with other actions that happen to it like "exhausted" or "removed from deck"?
        /// </summary>
        public abstract void OtherEffects(Action action, EffectSet ef, int upgradeCount);
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
        public override void OtherEffects(Action action, EffectSet ef, int upgradeCount) { }
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
        public override void OtherEffects(Action action, EffectSet ef, int upgradeCount) { }
    }
    public abstract class StatusCard : Card
    {
        public override CardType CardType => CardType.Status;
    }
    public abstract class EnemyCard : Card
    {
        public override CardType CardType => CardType.Attack;
        public override void OtherEffects(Action action, EffectSet ef, int upgradeCount) { }
    }
}
