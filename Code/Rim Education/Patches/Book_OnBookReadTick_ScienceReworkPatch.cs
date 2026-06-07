using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ScienceRework.Patches
{
    [HarmonyPatch(typeof(Book))]
    [HarmonyPatch("OnBookReadTick")]
    public static class Book_OnBookReadTick_ScienceReworkPatch
    {
        private static void Postfix(ref Book __instance, Pawn pawn, int delta, float roomBonusFactor)
        {
            if (!Utility.Settings.educationByReading)
            {
                return;
            }

            if (!pawn.TryGetEducation(out EducationDef education) || education.maxResearchLevel >= TechLevel.Industrial)
            {
                return;
            }

            // книгами в 2 раза медленнее обычного
            float factor = pawn.GetStatValue(StatDefOf.ReadingSpeed) * roomBonusFactor * (float)delta * 0.5f;

            float amount = Utility.BasicLearnXpPerTick * factor;

            pawn.TryLearnForEducation(pawn, amount);
        }
    }
}
