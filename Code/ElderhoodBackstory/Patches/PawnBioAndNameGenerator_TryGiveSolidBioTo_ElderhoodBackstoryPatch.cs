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

namespace ElderhoodBackstory.Patches
{
    [HarmonyPatch(typeof(PawnBioAndNameGenerator))]
    [HarmonyPatch("TryGiveSolidBioTo")]
    public class PawnBioAndNameGenerator_TryGiveSolidBioTo_ElderhoodBackstoryPatch
    {
        private static void Postfix(Pawn pawn, string requiredLastName, List<BackstoryCategoryFilter> backstoryCategories, ref bool __result)
        {
            if (__result)
            {
                Utility.FillBackstorySlotShuffled(pawn);
            }
        }
    }
}
