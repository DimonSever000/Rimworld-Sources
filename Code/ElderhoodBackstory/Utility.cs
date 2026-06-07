using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ElderhoodBackstory
{
    [StaticConstructorOnStartup]
    public static class Utility
    {
        private static List<BackstoryDef> tmpBackstories = new List<BackstoryDef>();

        private static readonly string elderhoodCategory = "Elderhood";

        private static List<BackstoryDef> elderhoods;
        public static List<BackstoryDef> Elderhoods
        {
            get
            {
                if (elderhoods == null)
                {
                    elderhoods = DefDatabase<BackstoryDef>.AllDefsListForReading.Where(x => x.IsElderhood()).ToList();
                }
                return elderhoods;
            }
        }

        public static bool IsElderhood(this BackstoryDef def)
        {
            if (def.spawnCategories == null)
            {
                return false;
            }

            return def.spawnCategories.Any(x => x == elderhoodCategory);
        }

        public static void FillBackstorySlotShuffled(Pawn pawn, Faction faction = null)
        {
            CompElderhoodBackstory compElderhoodBackstory = pawn.GetComp<CompElderhoodBackstory>();

            if (compElderhoodBackstory != null && pawn.ageTracker.AgeBiologicalYearsFloat >= compElderhoodBackstory.Props.elderhoodAge)
            {
                if (pawn.IsCreepJoiner)
                {
                    compElderhoodBackstory.Elderhood = BackstoryDefOfLocal.Elderhood_Unknown;
                    return;
                }

                IEnumerable<BackstoryDef> source = Elderhoods.Where(x => x != BackstoryDefOfLocal.Elderhood_PlayerColonist);

                tmpBackstories.Clear();

                tmpBackstories.AddRange(source);

                if (!(from bs in tmpBackstories.TakeRandom(20)
                      where (bs.requiredWorkTags == WorkTags.None || (!bs.requiredWorkTags.OverlapsWithOnAnyWorkType(pawn.story.Adulthood.workDisables) &&
                      !bs.requiredWorkTags.OverlapsWithOnAnyWorkType(pawn.story.Childhood.workDisables))) ? true : false
                      select bs).TryRandomElement(out var result))
                {
                    Log.Error(string.Concat($"No shuffled elderhood found for {pawn.ToStringSafe()}. Choosing random."));
                    result = Elderhoods.RandomElement();
                }

                compElderhoodBackstory.Elderhood = result;

                tmpBackstories.Clear();
            }
        }

        public static void FillBackstorySlotDirectlyForPlayer(Pawn pawn)
        {
            CompElderhoodBackstory compElderhoodBackstory = pawn.GetComp<CompElderhoodBackstory>();

            if (compElderhoodBackstory != null && pawn.ageTracker.AgeBiologicalYearsFloat >= compElderhoodBackstory.Props.elderhoodAge)
            {
                if (pawn.IsColonist)
                {
                    compElderhoodBackstory.Elderhood = BackstoryDefOfLocal.Elderhood_PlayerColonist;
                    return;
                }
                if (pawn.IsPrisonerOfColony)
                {
                    compElderhoodBackstory.Elderhood = BackstoryDefOfLocal.Elderhood_PlayerPrisoner;
                    return;
                }
                if (pawn.IsSlaveOfColony)
                {
                    compElderhoodBackstory.Elderhood = BackstoryDefOfLocal.Elderhood_PlayerSlave;
                    return;
                }

                compElderhoodBackstory.Elderhood = Elderhoods.RandomElement();
            }
        }
    }
}
