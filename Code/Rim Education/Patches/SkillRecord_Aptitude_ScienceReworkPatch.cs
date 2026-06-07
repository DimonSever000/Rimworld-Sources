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
    [HarmonyPatch(typeof(SkillRecord))]
    [HarmonyPatch("Aptitude", MethodType.Getter)]
    public static class SkillRecord_Aptitude_ScienceReworkPatch
    {
        private static bool recache;

        [HarmonyPrefix]
        private static void Prefix(ref SkillRecord __instance, ref int? ___aptitudeCached)
        {
            if (!___aptitudeCached.HasValue)
            {
                recache = true;
            }
        }

        [HarmonyPostfix]
        private static void Postfix(ref SkillRecord __instance, ref int __result, ref int? ___aptitudeCached, Pawn ___pawn)
        {
            if (!recache)
            {
                return;
            }

            if (___pawn.TryGetEducation(out EducationDef education))
            {
                if (!___aptitudeCached.HasValue)
                {
                    ___aptitudeCached = 0;
                }
                ___aptitudeCached += education.AptitudeFor(__instance.def);
            }

            recache = false;
        }
    }
}
