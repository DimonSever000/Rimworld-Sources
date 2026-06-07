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
    [HarmonyPatch(typeof(PawnBioAndNameGenerator))]
    [HarmonyPatch("BackstorySelectionWeight")]
    public class PawnBioAndNameGenerator_BackstorySelectionWeight_ElderhoodBackstoryPatch
    {
        private static void Postfix(BackstoryDef bs, ref float __result)
        {
            if (bs.IsElderhood())
            {
                __result = 0f;
            }
        }
    }
}
