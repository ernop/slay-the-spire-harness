using System.Collections.Generic;
using System.Linq;

namespace StS
{
    public class Havok : IroncladSkillCard
    {
        public override string Name => nameof(Havok);

        public override TargetType TargetType => TargetType.Player;

        public override int CiCanCallEnergyCost(int upgradeCount) => upgradeCount == 0 ? 1 : 0;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null)
        {
            var x = new OneEffect
            {
                Action = (Fight f, Deck d, List<string> history) =>
                {
                    var theCard = d.Draw(targetCards: null, count: 1, reshuffle: true, ef: ef, history: history).SingleOrDefault();
                    if (theCard == null)
                    {
                        return "Havok drew nothing";
                    }

                    f.PlayCard(theCard, forceExhaust: true, newCard: true, source: new List<CardInstance>() { theCard });
                    return $"Havok drew and played {theCard} which then exhausted";
                }
            };
            ef.FightEffect.Add(x);
        }
    }
}
