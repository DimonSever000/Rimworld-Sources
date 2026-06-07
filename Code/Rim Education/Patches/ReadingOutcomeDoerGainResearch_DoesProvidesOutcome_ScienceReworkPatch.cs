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
    [HarmonyPatch(typeof(ReadingOutcomeDoerGainResearch))]
    [HarmonyPatch("DoesProvidesOutcome")]
    public static class ReadingOutcomeDoerGainResearch_DoesProvidesOutcome_ScienceReworkPatch
    {
        private static void Postfix(ref ReadingOutcomeDoerGainResearch __instance, ref bool __result, Pawn reader, ref Dictionary<ResearchProjectDef, float> ___values)
        {
            if (!Utility.Settings.educationRestrictionsForBooks)
            {
                return;
            }

            if (__result)
            {
                if (reader != null)
                {
                    foreach (KeyValuePair<ResearchProjectDef, float> value in ___values)
                    {
                        if (!reader.CanResearch(value.Key))
                        {
                            __result = false;
                            return;
                        }
                    }
                }
            }
        }
    }
}
