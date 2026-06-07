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
    [HarmonyPatch("GenerateTraits")]
    public class PawnGenerator_GenerateTraits_ElderhoodBackstoryPatch
    {
        private static void Postfix(Pawn pawn, PawnGenerationRequest request)
        {
            CompElderhoodBackstory compElderhoodBackstory = pawn.GetComp<CompElderhoodBackstory>();

            if (compElderhoodBackstory != null && compElderhoodBackstory.Elderhood != null && !compElderhoodBackstory.Elderhood.forcedTraits.NullOrEmpty())
            {
                List<BackstoryTrait> forcedTraits = compElderhoodBackstory.Elderhood.forcedTraits;
                for (int i = 0; i < forcedTraits.Count; i++)
                {
                    BackstoryTrait te = forcedTraits[i];
                    if (te.def == null)
                    {
                        Log.Error("Null forced trait def on " + pawn.story.Adulthood);
                    }
                    else if (!request.KindDef.disallowedTraits.NotNullAndContains(te.def) && (request.KindDef.disallowedTraitsWithDegree == null || !request.KindDef.disallowedTraitsWithDegree.Any((TraitRequirement t) => t.def == te.def && !t.degree.HasValue)) && !pawn.story.traits.HasTrait(te.def) && (request.ProhibitedTraits == null || !request.ProhibitedTraits.Contains(te.def)))
                    {
                        pawn.story.traits.GainTrait(new Trait(te.def, te.degree));
                    }
                }
            }
        }
    }
}
