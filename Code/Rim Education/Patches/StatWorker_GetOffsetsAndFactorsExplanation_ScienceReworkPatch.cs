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
using static UnityEngine.ParticleSystem;

namespace ScienceRework.Patches
{
    [HarmonyPatch(typeof(StatWorker))]
    [HarmonyPatch("GetOffsetsAndFactorsExplanation")]
    public static class StatWorker_GetOffsetsAndFactorsExplanation_ScienceReworkPatch
    {
        private static void Postfix(ref StatWorker __instance, StatRequest req, ref StringBuilder sb, float baseValue, string whitespace, StatDef ___stat)
        {
            Pawn pawn = req.Thing as Pawn;

            if (pawn != null)
            {
                if (pawn.TryGetEducation(out EducationDef education) && !education.statFactors.NullOrEmpty())
                {
                    StatModifier statModifier = education.statFactors.FirstOrDefault((StatModifier stat) => stat.stat == ___stat);
                    if (statModifier != null)
                    {
                        sb.AppendLine(whitespace + "ScienceRework.StatWorker_GetOffsetsAndFactorsExplanation_ScienceReworkPatch.StatsReport_Education".Translate());
                        sb.AppendLine(whitespace + "    " + education.LabelCap + ": " + statModifier.ToStringAsFactor);
                    }
                }
            }
        }
    }
}
