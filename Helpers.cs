﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace StS
{
    public static class Helpers
    {
        public static string Output = "C:/dl/output.txt";
        public static List<FightActionEnum> RoundEndConditions = new List<FightActionEnum>() { FightActionEnum.EndTurn, FightActionEnum.WonFight, FightActionEnum.LostFight, FightActionEnum.TooLong };
        public static Random Rnd { get; private set; }
        public static void SetRandom(int seed)
        {
            Rnd = new Random(seed);
        }

        public static bool IsVulnerable(Entity entity)
        {
            var exi = entity.StatusInstances.SingleOrDefault(el => el.Status.StatusType == StatusType.Vulnerable);
            if (exi != null)
            {
                return true;
            }
            return false;
        }

        internal static Tuple<string, int> SplitCardName(string name)
        {
            //for now just detect trailing+
            var upgradeCount = 0;
            if (name.EndsWith("+"))
            {
                upgradeCount = 1;
                name = name.TrimEnd('+');
            }
            return new Tuple<string, int>(name, upgradeCount);
        }

        public static bool CompareStatuses(List<StatusInstance> a, List<StatusInstance> b, out string error)
        {

            if (a.Count != b.Count)
            {
                error = "Status length mismatch";
                return false;
            }

            a = a.OrderBy(el => el.Status.StatusType).ToList();
            b = b.OrderBy(el => el.Status.StatusType).ToList();

            for (var i = 0; i < a.Count; i++)
            {
                var ael = a[i];
                var bel = b[i];
                if (ael.Status.StatusType != bel.Status.StatusType)
                {
                    error = "Mismatching status";
                    return false;
                }
                if (ael.Intensity != bel.Intensity)
                {
                    error = "Mismatch intensity";
                    return false;
                }
                if (ael.Duration != bel.Duration)
                {
                    error = "Mismatch duration";
                    return false;
                }
            }
            error = "Okay";
            return true;
        }

        public static List<int> Repeat(int num, int count)
        {
            var ii = 0;
            var res = new List<int>();
            while (ii < count)
            {
                res.Add(num);
                ii++;
            }
            return res;
        }

        /// <summary>
        /// sometimes we'll have target effects (gaining block from playing "defend")
        /// and source effects (afterimage is a status that gives +1 block per card played)
        /// When target == source, we have to combine them.
        /// </summary>
        public static IndividualEffect Combine(IndividualEffect ef1, IndividualEffect ef2)
        {
            var combined = new IndividualEffect();
            combined.InitialBlock = ef1.InitialBlock + ef2.InitialBlock;
            var res = new List<int>();
            if (ef1.InitialDamage != null || ef2.InitialDamage != null)
            {
                foreach (var ls in new List<IEnumerable<int>>() { ef1.InitialDamage, ef2.InitialDamage })
                {
                    foreach (var el in ls)
                    {
                        res.Add(el);
                    }
                }
                combined.InitialDamage = res;
            }

            combined.DamageAdjustments.AddRange(ef1.DamageAdjustments);
            combined.DamageAdjustments.AddRange(ef2.DamageAdjustments);
            combined.BlockAdjustments.AddRange(ef1.BlockAdjustments);
            combined.BlockAdjustments.AddRange(ef2.BlockAdjustments);
            combined.Status.AddRange(ef1.Status);
            combined.Status.AddRange(ef2.Status);
            return combined;
        }

        /// <summary>
        /// meant to be a complete comparison of per-hand lists of CIs.
        /// </summary>
        public static bool CompareHands(IList<CardInstance> a, IList<CardInstance> b, out string message)
        {
            if (a.Count() != b.Count())
            {
                message = "length mismatch";
                return false;
            }
            var al = string.Join(',', a.Select(el => el.ToString()).OrderBy(el => el));
            var bl = string.Join(',', b.Select(el => el.ToString()).OrderBy(el => el));
            if (al != bl)
            {
                message = $"{al} != {bl}";
                return false;
            }

            message = "okay";
            return true;
        }

        public static CardInstance CopyCI(CardInstance ci)
        {
            var newCi = new CardInstance(ci.Card, ci.UpgradeCount);
            return newCi;
        }

        public static IEnumerable<CardInstance> GetNonZeroCostsFromHand(IList<CardInstance> hand)
        {
            var nonZeros = hand.Where(el => el.EnergyCost() > 0);
            return nonZeros;
        }

        /// <summary>
        /// Null or a CI that has nonzero cost
        /// </summary>
        public static CardInstance SelectNonZeroCostCard(IList<CardInstance> hand)
        {
            var nonZeros = GetNonZeroCostsFromHand(hand);
            var len = nonZeros.Count();
            if (len == 0)
            {
                return null;
            }
            var num = Helpers.Rnd.Next(len);
            return nonZeros.Skip(num).First();
        }


        public static CardInstance GetCi(string name)
        {
            var x = SplitCardName(name);
            var card = AllCards.Cards[x.Item1];
            if (card == null)
            {
                throw new Exception("Missing card.");
            }

            var ci = new CardInstance(card, x.Item2);
            return ci;
        }

        public static List<CardInstance> GetCis(params string[] names)
        {
            var cis = new List<CardInstance>();
            foreach (var name in names)
            {
                var x = SplitCardName(name);
                var card = AllCards.Cards[x.Item1];
                if (card == null)
                {
                    throw new Exception("Missing card.");
                }

                var ci = new CardInstance(card, x.Item2);
                cis.Add(ci);
            }
            return cis;
        }

        public static List<Relic> GetRelics(params string[] relics)
        {
            var res = new List<Relic>();
            foreach (var x in relics)
            {
                res.Add(AllRelics.Relics[x].Copy());
            }
            return res;
        }

        public static EnemyAttack GetAttack(int amount, int count)
        {
            return new EnemyAttack(amount, count);
        }

        /// <summary>
        /// Generate all sets of startinghands from a list of cis.
        /// 
        /// some uncertainty here if I should just gen unique subsets.
        /// But I think for v1 just pick a straight combination
        /// 
        /// </summary>
        public static List<List<CardInstance>> GenSubsets(IEnumerable<CardInstance> cis, int n)
        {
            var res = new List<List<CardInstance>>();
            if (n == 1)
            {
                res.AddRange(cis.Select(el => new List<CardInstance>() { el.Copy() }));
            }
            else
            {
                foreach (var i in Enumerable.Range(1, cis.Count()))
                {
                    var subcis = cis.Skip(i);
                    if (subcis.Count() < n - 1)
                    {
                        continue;
                    }
                    var subres = GenSubsets(subcis, n - 1);
                    foreach (var su in subres)
                    {
                        su.Insert(0, cis.Skip(i - 1).First());
                        res.Add(su);

                    }
                }
            }
            return res;
        }

        public class Counter
        {
            public List<CardInstance> Cis { get; set; }
            public string Key { get; set; }
            public int Count { get; set; }
            public Counter(List<CardInstance> cis)
            {
                Key = string.Join(',', cis.Select(el => el.ToString()).OrderBy(el => el));
                Cis = cis;
                Count = 0;
            }
        }

        /// <summary>
        /// For a given set of draws, with repeats, weigh them by frequency
        /// </summary>
        public static List<Tuple<List<CardInstance>, int>> GenHandWeights(List<List<CardInstance>> startingHands)
        {
            var counters = new Dictionary<string, Counter>();
            foreach (var sh in startingHands)
            {
                var key = string.Join(',', sh.Select(el => el.ToString()).OrderBy(el => el));
                var counter = new Counter(sh);
                if (!counters.ContainsKey(key))
                {
                    counters[key] = counter;
                }
                counters[key].Count++;
            }
            var res = new List<Tuple<List<CardInstance>, int>>();
            foreach (var k in counters.Keys)
            {
                res.Add(new Tuple<List<CardInstance>, int>(counters[k].Cis, counters[k].Count));
            }

            return res;

        }

        public static CardInstance FindIdenticalCardInSource(IList<CardInstance> source, CardInstance card, IList<CardInstance> exclusions = null)
        {
            if (exclusions == null)
            {
                exclusions = new List<CardInstance>();
            }
            CardInstance res = null;
            var cs = card.ToString();
            foreach (var c in source)
            {
                if (!exclusions.Contains(c) && c.ToString() == cs)
                {
                    return c;
                }
            }
            if (res == null)
            {
                throw new Exception("No card.");
            }
            return res;
        }

        public static List<StatusInstance> GetStatuses(Status status, int num)
        {
            return new List<StatusInstance>() { new StatusInstance(status, num) };
        }

        public static FightNode GetBestLeaf(FightNode f)
        {
            if (f.Choices.Count == 0)
            {
                if (f.Randoms.Count == 1)
                {
                    return GetBestLeaf(f.Randoms.First());
                }
            }
            var res = f;
            while (true)
            {
                var bc = res.BestChild();
                if (bc == null)
                {
                    break;
                }
                res = bc;
            }
            if (res.Randoms.Count == 0)
            {
                return res;
            }
            return GetBestLeaf(res);
        }

    }
}
