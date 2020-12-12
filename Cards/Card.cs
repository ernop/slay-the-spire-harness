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
        internal abstract void Play(EffectSet ef, IEntity source, IEntity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null);
        internal virtual bool Exhausts(int upgradeCount) => false;
        internal virtual bool Ethereal(int upgradeCount) => false;

        /// <summary>
        /// Only callable from cardInstance, to detect per-fight and per-hand cost changes.
        /// </summary>
        public abstract int CiCanCallEnergyCost(int upgradeCount);

        public virtual bool Playable(List<CardInstance> hand) { return true; }

        /// <summary>
        /// Does the card need to be called with other actions that happen to it like "exhausted" or "removed from deck"?
        /// </summary>
        public virtual void OtherAction(Action action, EffectSet ef, int upgradeCount) { }
        public virtual bool Playable(IList<CardInstance> hand) { return true; }

        /// <summary>
        /// the ai will call getTargets and then if randomtarget, pick one; if not, all are considered possible.
        /// </summary>
        public override string ToString()
        {
            return Name;
        }
    }


    public abstract class IroncladSkillCard : SkillCard
    {
        public override CharacterType CharacterType => CharacterType.IronClad;
    }

    public abstract class SkillCard : Card
    {
        public override CardType CardType => CardType.Skill;
    }

    public abstract class IroncladPowerCard : PowerCard
    {
        public override CharacterType CharacterType => CharacterType.IronClad;
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
        public override CharacterType CharacterType => CharacterType.Enemy;
    }
}
