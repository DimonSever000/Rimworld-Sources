using HarmonyLib;
using RimWorld;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld.Planet;
using EventsCore.Utilities;

namespace EventsCore.Patches
{
    /// <summary>
    /// Отвечает за уровень освещения на карте игрока
    /// </summary>
    [HarmonyPatch(typeof(GenCelestial))]
    [HarmonyPatch("CelestialSunGlowPercent")]
    public class GenCelestial_CelestialSunGlowPercent_EventsCorePatch
    {
        private static void Postfix(ref float __result)
        {
            __result = __result * MiscUtility.CurSunLuminosity;
        }
    }
}
