using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ScienceRework.Patches
{
    [HarmonyPatch(typeof(LearningUtility))]
    [HarmonyPatch("LearningTickCheckEnd")]
    public static class LearningUtility_LearningTickCheckEnd_ScienceReworkPatch
    {
        private static bool Prefix(Pawn pawn, int delta, bool forced)
        {
            Job job = pawn.jobs?.curJob;
            if (job != null && job.def == JobDefOf.Lessontaking)
            {
                Pawn teacher = job.GetTarget(TargetIndex.B).Pawn;
                if (teacher != null)
                {
                    float amount = Utility.BasicLearnXpPerTick * delta;
                    Utility.TryLearnForEducation(pawn, teacher, amount);
                }
            }

            return true;
        }
    }
}
