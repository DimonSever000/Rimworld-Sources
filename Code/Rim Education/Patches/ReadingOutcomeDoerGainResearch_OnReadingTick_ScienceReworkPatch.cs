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
using static UnityEngine.Networking.UnityWebRequest;

namespace ScienceRework.Patches
{
    [HarmonyPatch(typeof(ReadingOutcomeDoerGainResearch))]
    [HarmonyPatch("OnReadingTick")]
    public static class ReadingOutcomeDoerGainResearch_OnReadingTick_ScienceReworkPatch
    {
        private static bool Prefix(ref ReadingOutcomeDoerGainResearch __instance, Pawn reader, float factor, ref Dictionary<ResearchProjectDef, float> ___values)
        {
            if (!Utility.Settings.educationRestrictionsForBooks)
            {
                return true;
            }

            if (reader != null)
            {
                foreach (KeyValuePair<ResearchProjectDef, float> value in ___values)
                {
                    if (!reader.CanResearch(value.Key))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
