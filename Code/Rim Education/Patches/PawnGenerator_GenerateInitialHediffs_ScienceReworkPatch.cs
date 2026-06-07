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
    [HarmonyPatch(typeof(PawnGenerator))]
    [HarmonyPatch("GenerateInitialHediffs")]
    public static class PawnGenerator_GenerateInitialHediffs_ScienceReworkPatch
    {
        private static void Postfix(Pawn pawn, PawnGenerationRequest request)
        {
            if (Utility.TryGenerateEducationFor(pawn, out EducationDef education))
            {
                pawn.TrySetEducation(education);
            }
        }
    }
}
