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

namespace ElderhoodBackstory.Patches
{
    [HarmonyPatch(typeof(Pawn_StoryTracker))]
    [HarmonyPatch("DisabledWorkTagsBackstoryAndTraits", MethodType.Getter)]
    public class Pawn_StoryTracker_DisabledWorkTagsBackstoryAndTraits_ElderhoodBackstoryPatch
    {
        private static void Postfix(ref WorkTags __result, ref Pawn ___pawn)
        {
            CompElderhoodBackstory compElderhoodBackstory = ___pawn.GetComp<CompElderhoodBackstory>();
            if (compElderhoodBackstory != null && compElderhoodBackstory.Elderhood != null)
            {
                __result = __result |= compElderhoodBackstory.Elderhood.workDisables;
            }
        }
    }
}
