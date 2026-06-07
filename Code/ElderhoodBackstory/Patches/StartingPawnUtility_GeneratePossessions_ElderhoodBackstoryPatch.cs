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
    [HarmonyPatch(typeof(StartingPawnUtility))]
    [HarmonyPatch("GeneratePossessions")]
    public class StartingPawnUtility_GeneratePossessions_ElderhoodBackstoryPatch
    {
        //private static FieldInfo StartingPossessions = AccessTools.Field(typeof(StartingPawnUtility), "StartingPossessions");
        private static void Postfix(Pawn pawn)
        {
            if (Find.GameInitData.startingPossessions[pawn].Count >= 2)
            {
                return;
            }

            if (Rand.Value < 1f)
            {
                CompElderhoodBackstory compElderhoodBackstory = pawn.GetComp<CompElderhoodBackstory>();
                if (compElderhoodBackstory != null && compElderhoodBackstory.Elderhood != null && !compElderhoodBackstory.Elderhood.possessions.NullOrEmpty())
                {
                    foreach (PossessionThingDefCountClass possession in compElderhoodBackstory.Elderhood.possessions)
                    {
                        if (Find.GameInitData.startingPossessions[pawn].Count >= 2)
                        {
                            return;
                        }
                        Find.GameInitData.startingPossessions[pawn].Add(new ThingDefCount(possession.key, Mathf.Clamp(possession.value.RandomInRange, 1, possession.key.stackLimit)));
                    }
                }
            }
        }
    }
}
