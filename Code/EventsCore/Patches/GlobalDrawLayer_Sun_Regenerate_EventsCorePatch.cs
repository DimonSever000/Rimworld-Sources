using HarmonyLib;
using RimWorld;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld.Planet;
using static RimWorld.IdeoFoundation_Deity;
using UnityEngine;
using System.Reflection;
using System.Collections;
using EventsCore.Utilities;

namespace EventsCore.Patches
{
    [HarmonyPatch(typeof(GlobalDrawLayer_Sun))]
    [HarmonyPatch("Regenerate")]
    public class GlobalDrawLayer_Sun_Regenerate_EventsCorePatch
    {
        private static readonly MethodInfo ClearSubMeshes = AccessTools.Method(typeof(GlobalDrawLayer_Sun), "ClearSubMeshes");
        private static readonly MethodInfo GetSubMesh = AccessTools.Method(typeof(GlobalDrawLayer_Sun), "GetSubMesh", new Type[] { typeof(Material) });
        private static readonly MethodInfo FinalizeMesh = AccessTools.Method(typeof(GlobalDrawLayer_Sun), "FinalizeMesh");
        private static bool Prefix(ref bool ___dirty, ref GlobalDrawLayer_Sun __instance, ref IEnumerable __result)
        {
            ___dirty = false;

            ClearSubMeshes.Invoke(__instance, new object[] { MeshParts.All });

            Rand.PushState();
            Rand.Seed = Find.World.info.Seed;

            LayerSubMesh subMesh = (LayerSubMesh)GetSubMesh.Invoke(__instance, new object[] { WorldMaterials.Sun });
            WorldRendererUtility.PrintQuadTangentialToPlanet(Vector3.forward * 10f, 15f * MiscUtility.CurSunLuminosity, 0f, subMesh, counterClockwise: true);

            Rand.PopState();
            FinalizeMesh.Invoke(__instance, new object[] { MeshParts.All });

            __result = Array.Empty<object>();

            return false;
        }
    }
}
