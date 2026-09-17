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
using EventsCore.Settings;

namespace EventsCore.Patches
{
    [HarmonyPatch(typeof(IncidentWorker))]
    [HarmonyPatch("CanFireNow")]
    public class IncidentWorker_CanFireNow_EventsCorePatch
    {
        private static void Postfix(ref bool __result, ref IncidentWorker __instance, IncidentParms parms)
        {
            if (__result)
            {
                if (!parms.forced)
                {
                    if (!EventsCoreMod.Settings.TryGetSettings<IncidentAllowedSettings>().IsIncidentAllowed(__instance.def))
                    {
                        __result = false;
                    }
                }
            }
        }
    }
}
