using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Verse;
using static UnityEngine.ParticleSystem;
using static Verse.MathEvaluatorCustomFunctions;

namespace ElderhoodBackstory.Patches
{
    [HarmonyPatch(typeof(PawnGenerator))]
    [HarmonyPatch("FinalLevelOfSkill")]
    public class PawnGenerator_FinalLevelOfSkill_ElderhoodBackstoryPatch
    {
        private static void Postfix(Pawn pawn, SkillDef sk, PawnGenerationRequest request, ref int __result)
        {
            CompElderhoodBackstory compElderhoodBackstory = pawn.GetComp<CompElderhoodBackstory>();

            if (compElderhoodBackstory != null && compElderhoodBackstory.Elderhood != null && !compElderhoodBackstory.Elderhood.skillGains.NullOrEmpty())
            {
                foreach (SkillGain skillGain in compElderhoodBackstory.Elderhood.skillGains)
                {
                    if (skillGain.skill == sk)
                    {
                        __result = Mathf.Clamp(__result + skillGain.amount, 0, 20);
                    }
                }
            }
        }
    }
}
