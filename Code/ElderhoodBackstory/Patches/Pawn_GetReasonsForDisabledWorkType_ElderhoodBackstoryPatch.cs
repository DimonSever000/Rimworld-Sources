using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Analytics;
using Verse;
using static UnityEngine.ParticleSystem;
using static Verse.MathEvaluatorCustomFunctions;

namespace ElderhoodBackstory.Patches
{
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("GetReasonsForDisabledWorkType")]
    public class Pawn_GetReasonsForDisabledWorkType_ElderhoodBackstoryPatch
    {
        private static void Postfix(ref Pawn __instance, WorkTypeDef workType, ref List<string> __result)
        {
            CompElderhoodBackstory compElderhoodBackstory = __instance.GetComp<CompElderhoodBackstory>();
            if (compElderhoodBackstory != null && compElderhoodBackstory.Elderhood == null)
            {
                foreach (WorkTypeDef disabledWorkType in compElderhoodBackstory.Elderhood.DisabledWorkTypes)
                {
                    if (workType == disabledWorkType)
                    {
                        __result.Add("WorkDisabledByBackstory".Translate(compElderhoodBackstory.Elderhood.TitleCapFor(__instance.gender)));
                        break;
                    }
                }
            }
        }
    }
}
