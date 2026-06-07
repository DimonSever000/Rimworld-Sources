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
    [HarmonyPatch(typeof(StatWorker))]
    [HarmonyPatch("GetValueUnfinalized")]
    public static class StatWorker_GetValueUnfinalized_ScienceReworkPatch
    {
        private static void Postfix(ref StatWorker __instance, ref float __result, ref StatRequest req, ref bool applyPostProcess, ref StatDef ___stat)
        {
            Pawn pawn = req.Thing as Pawn;

            if (pawn != null)
            {
                if (pawn.TryGetEducation(out EducationDef education))
                {
                    __result = __result * education.MultiplierOfStat(___stat);
                }
            }
        }
    }
}
