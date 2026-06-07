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
    [HarmonyPatch(typeof(ExpectationsUtility))]
    [HarmonyPatch("CurrentExpectationFor", new Type[] { typeof(Pawn) })]
    public static class ExpectationsUtility_CurrentExpectationFor_ScienceReworkPatch
    {
        private static void Postfix(ref ExpectationDef __result, Pawn p)
        {
            if (Utility.TryGetCurrentExpectationFor(p, __result, out ExpectationDef expectation))
            {
                __result = expectation;
            }
        }
    }
}
