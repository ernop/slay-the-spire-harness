using System.Collections.Generic;
using System.Linq;

namespace StS
{
    public class Headbutt : AttackCard
    {
        public override string Name => nameof(Headbutt);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override int CiCanCallEnergyCost(int upgradeCount) => 1;

        public override bool Ethereal(int upgradeCount) => false;

        public override bool Exhausts(int upgradeCount) => false;

        /// <summary>
        /// There needs to be a way to specify extra parameters to play, for things like:
        /// TrueGrit, etc.
        /// </summary>
        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount, List<CardInstance> targets = null)
        {
            int dmg;
            if (upgradeCount == 0)
            {
                dmg = 9;
            }
            else
            {
                dmg = 12;
            }
            ef.TargetEffect.InitialDamage = new List<int>() { dmg };

            ef.DeckEffect.Add((Deck deck) =>
            {
                var headbuttTarget = targets.First();
                if (!deck.DiscardPile.Contains(headbuttTarget))
                {
                    throw new System.Exception("Trying to headbutt card not in discard.");
                }
                deck.DiscardPile.Remove(headbuttTarget);
                deck.DrawPile.Add(headbuttTarget);
            });
        }
    }
}
