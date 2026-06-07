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
    [HarmonyPatch("GetBenefitsString")]
    public static class ReadingOutcomeDoerGainResearch_GetBenefitsString_ScienceReworkPatch
    {
        private static void Postfix(ref ReadingOutcomeDoerGainResearch __instance, ref string __result, Pawn reader, ref Dictionary<ResearchProjectDef, float> ___values)
        {
            string extraDesc = string.Empty;

            if (Utility.Settings.educationRestrictionsForBooks)
            {
                EducationDef education = null;

                foreach (KeyValuePair<ResearchProjectDef, float> value in ___values)
                {
                    if (Utility.MinEducationForTechLevelDict.TryGetValue(value.Key.techLevel, out EducationDef def))
                    {
                        if (education == null || def.maxResearchLevel > education.maxResearchLevel)
                        {
                            education = def;
                        }
                    }
                }

                if (education != null)
                {
                    extraDesc = "ScienceRework.ReadingOutcomeDoerGainResearch_GetBenefitsString_ScienceReworkPatch.ReqEducation".Translate(education.label.Colorize(ColoredText.TipSectionTitleColor)).Resolve();
                }
            }

            if (Utility.Settings.educationByReading)
            {
                if (Utility.MinEducationForTechLevelDict.TryGetValue(TechLevel.Industrial, out EducationDef def))
                {
                    if (!extraDesc.NullOrEmpty())
                    {
                        extraDesc += "\n";
                    }

                    extraDesc += "ScienceRework.ReadingOutcomeDoerGainResearch_GetBenefitsString_ScienceReworkPatch.EducationByReading".Translate(def.label.Colorize(ColoredText.TipSectionTitleColor)).Resolve();
                }
            }

            if (!extraDesc.NullOrEmpty())
            {
                __result = __result + "\n\n" + extraDesc;
            }
        }
    }
}
