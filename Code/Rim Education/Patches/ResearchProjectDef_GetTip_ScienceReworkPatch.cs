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
    [HarmonyPatch(typeof(ResearchProjectDef))]
    [HarmonyPatch("GetTip")]
    public static class ResearchProjectDef_GetTip_ScienceReworkPatch
    {
        private static bool recache;

        [HarmonyPrefix]
        private static void Prefix(ref ResearchProjectDef __instance, ref string ___cachedDescription)
        {
            if (___cachedDescription == null)
            {
                recache = true;
            }
        }

        [HarmonyPostfix]
        private static void Postfix(ref ResearchProjectDef __instance, ref string ___cachedDescription)
        {
            if (!recache)
            {
                return;
            }

            if (!Utility.Settings.educationRestrictionsForResearch)
            {
                return;
            }

            TechLevel techLevel = __instance.GetResearchTechLevel();

            if (Utility.MinEducationForTechLevelDict.TryGetValue(techLevel, out EducationDef education))
            {
                string extraDesc = "\n\n" + ("ScienceRework.ResearchProjectDef_GetTip_ScienceReworkPatch.MinEducation".Translate(education.label.Colorize(ColoredText.TipSectionTitleColor)).Resolve());
                ___cachedDescription = ___cachedDescription + extraDesc;
            }

            recache = false;
        }
    }
}
