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
using Verse.AI;

namespace ScienceRework.Patches
{
    [HarmonyPatch(typeof(JobDriver_Research))]
    [HarmonyPatch("MakeNewToils")]
    public static class JobDriver_Research_MakeNewToils_ScienceReworkPatch
    {
        private static IEnumerable<Toil> Postfix(IEnumerable<Toil> __result, JobDriver_Research __instance)
        {
            foreach (Toil toil in __result)
            {
                if (toil.debugName == "MakeNewToils")
                {
                    toil.FailOn(() => !toil.actor.CanResearch(Find.ResearchManager.GetProject()));
                }

                yield return toil;
            }
        }
    }
}
