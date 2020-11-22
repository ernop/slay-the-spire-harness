using System.Collections.Generic;

namespace StS
{
    public abstract class Card
    {
        public abstract string Name { get; }
        public string Text { get; }
        public abstract CharacterType CharacterType { get; }
        public abstract CardType CardType { get; }
        public abstract TargetType TargetType { get; }
        internal abstract void Play(EffectSet ef, Entity source, Entity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null);
        public virtual bool Exhausts(int upgradeCount) => false;
        public virtual bool Ethereal(int upgradeCount) => false;

        /// <summary>
        /// Only callable from cardInstance, to detect per-fight and per-hand cost changes.
        /// </summary>
        public abstract int CiCanCallEnergyCost(int upgradeCount);

        /// <summary>
        /// Does the card need to be called with other actions that happen to it like "exhausted" or "removed from deck"?
        /// </summary>
        public virtual void OtherEffects(Action action, EffectSet ef, int upgradeCount) { }
        public virtual bool Playable(List<CardInstance> hand) { return true; }

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

    public abstract class IroncladAttackCard : AttackCard
    {
        public override CharacterType CharacterType => CharacterType.IronClad;
    }

    public abstract class AttackCard : Card
    {
        public override TargetType TargetType => TargetType.Enemy;
        public override CardType CardType => CardType.Attack;

    }
    public abstract class StatusCard : Card
    {
        public override CardType CardType => CardType.Status;
    }
    public abstract class EnemyCard : Card
    {
        public override CardType CardType => CardType.Attack;
    }
}
