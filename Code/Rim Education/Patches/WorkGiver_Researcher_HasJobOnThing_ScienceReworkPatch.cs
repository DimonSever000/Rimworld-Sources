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
    [HarmonyPatch(typeof(WorkGiver_Researcher))]
    [HarmonyPatch("HasJobOnThing")]
    public static class WorkGiver_Researcher_HasJobOnThing_ScienceReworkPatch
    {
        private static void Postfix(ref WorkGiver_Researcher __instance, ref bool __result, Pawn pawn, Thing t, bool forced)
        {
            if (__result)
            {
                ResearchProjectDef project = Find.ResearchManager.GetProject();

                if (project != null)
                {
                    if (!pawn.CanResearch(project))
                    {
                        __result = false;
                    }
                }
            }
        }
    }
}
