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
    [HarmonyPatch(typeof(GameComponent_PawnDuplicator))]
    [HarmonyPatch("CopyStoryAndTraits")]
    public class CopyStoryAndTraits_ElderhoodBackstoryPatch
    {
        private static void Postfix(Pawn pawn, Pawn newPawn)
        {
            CompElderhoodBackstory comp1 = pawn.GetComp<CompElderhoodBackstory>();
            CompElderhoodBackstory comp2 = newPawn.GetComp<CompElderhoodBackstory>();

            if (comp1 != null && comp2 != null)
            {
                comp2.Elderhood = comp1.Elderhood;
            }
        }
    }
}
