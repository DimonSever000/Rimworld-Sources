using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static UnityEngine.ParticleSystem;
using static Verse.MathEvaluatorCustomFunctions;

namespace ElderhoodBackstory.Patches
{
    [HarmonyPatch(typeof(CharacterCardUtility))]
    [HarmonyPatch("GetWorkTypeDisableCauses")]
    public class CharacterCardUtility_GetWorkTypeDisableCauses_ElderhoodBackstoryPatch
    {
        private static void Postfix(Pawn pawn, WorkTags workTag, ref List<object> __result)
        {
            CompElderhoodBackstory compElderhoodBackstory = pawn.GetComp<CompElderhoodBackstory>();

            if (compElderhoodBackstory != null && compElderhoodBackstory.Elderhood != null && (compElderhoodBackstory.Elderhood.workDisables & workTag) != 0)
            {
                __result.Add(compElderhoodBackstory.Elderhood);
            }
        }
    }
}
