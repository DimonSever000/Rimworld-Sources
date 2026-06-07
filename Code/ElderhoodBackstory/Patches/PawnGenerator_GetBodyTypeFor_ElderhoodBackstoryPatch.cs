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
    [HarmonyPatch(typeof(PawnGenerator))]
    [HarmonyPatch("GetBodyTypeFor")]
    public class PawnGenerator_GetBodyTypeFor_ElderhoodBackstoryPatch
    {
        private static HashSet<BodyTypeDef> tmpBodyTypes = new HashSet<BodyTypeDef>();
        private static void Postfix(Pawn pawn, ref BodyTypeDef __result)
        {
            if (ModsConfig.BiotechActive && pawn.DevelopmentalStage.Juvenile())
            {
                return;
            }

            if (ModsConfig.BiotechActive && pawn.genes != null)
            {
                List<Gene> genesListForReading = pawn.genes.GenesListForReading;
                for (int i = 0; i < genesListForReading.Count; i++)
                {
                    if (genesListForReading[i].def.bodyType.HasValue)
                    {
                        tmpBodyTypes.Add(genesListForReading[i].def.bodyType.Value.ToBodyType(pawn));
                    }
                }

                if (tmpBodyTypes.TryRandomElement(out var result))
                {
                    tmpBodyTypes.Clear();
                    return;
                }

                tmpBodyTypes.Clear();
            }

            CompElderhoodBackstory compElderhoodBackstory = pawn.GetComp<CompElderhoodBackstory>();
            if (compElderhoodBackstory != null && compElderhoodBackstory.Elderhood != null)
            {
                BodyTypeDef bodyTypeDef = compElderhoodBackstory.Elderhood.BodyTypeFor(pawn.gender);
                if (bodyTypeDef != null)
                {
                    __result = bodyTypeDef;
                }
            }
        }
    }
}
