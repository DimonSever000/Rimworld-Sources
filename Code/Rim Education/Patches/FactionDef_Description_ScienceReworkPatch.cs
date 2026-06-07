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
    [HarmonyPatch(typeof(FactionDef))]
    [HarmonyPatch("Description", MethodType.Getter)]
    public static class FactionDef_Description_ScienceReworkPatch
    {
        private static bool recache;

        [HarmonyPrefix]
        private static void Prefix(ref FactionDef __instance, ref string ___cachedDescription)
        {
            if (___cachedDescription == null)
            {
                recache = true;
            }
        }

        [HarmonyPostfix]
        private static void Postfix(ref FactionDef __instance, ref string ___cachedDescription)
        {
            if (!recache)
            {
                return;
            }

            if (!__instance.humanlikeFaction)
            {
                return;
            }

            EducationSet educationSet = __instance.GetModExtension<DefModExtension_EducationSettings>()?.educationSet;
            if (educationSet == null || educationSet.educationChances.NullOrEmpty())
            {
                Utility.EducationSetForFactionByDefault.TryGetValue(__instance.techLevel, out educationSet);
            }

            if (educationSet == null || educationSet.educationChances.NullOrEmpty())
            {
                return;
            }

            List<EducationChance> list = educationSet.educationChances.ToList();
            if (!list.NullOrEmpty())
            {
                list.SortBy(x => 0f - x.chance);
                string extraDesc = "\n\n" + ("ScienceRework.FactionDef_Description_ScienceReworkPatch.Educations".Translate() + ":").AsTipTitle() + "\n"
                    + list.Select((EducationChance x) => $"{x.education.LabelCap}: {Mathf.Min(x.chance, 1f).ToStringPercent()}").ToLineList("  - ");
                ___cachedDescription = ___cachedDescription + extraDesc;
            }

            recache = false;
        }
    }
}
