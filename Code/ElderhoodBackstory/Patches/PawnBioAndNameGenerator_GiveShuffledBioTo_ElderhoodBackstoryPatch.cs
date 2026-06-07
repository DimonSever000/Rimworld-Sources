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
using static UnityEngine.ParticleSystem;
using static Verse.MathEvaluatorCustomFunctions;

namespace ElderhoodBackstory.Patches
{
    [HarmonyPatch(typeof(PawnBioAndNameGenerator))]
    [HarmonyPatch("GiveShuffledBioTo")]
    public class PawnBioAndNameGenerator_GiveShuffledBioTo_ElderhoodBackstoryPatch
    {
        private static void Postfix(Pawn pawn, FactionDef factionType, string requiredLastName, List<BackstoryCategoryFilter> backstoryCategories, bool forceNoBackstory, bool forceNoNick, XenotypeDef xenotype, bool onlyForcedBackstories)
        {
            if (!forceNoBackstory)
            {
                Utility.FillBackstorySlotShuffled(pawn);
            }
        }
    }
}
