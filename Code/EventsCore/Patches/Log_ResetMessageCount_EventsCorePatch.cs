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
using System.Reflection;
using EventsCore.Settings;

namespace EventsCore.Patches
{
    [HarmonyPatch(typeof(Log))]
    [HarmonyPatch("ResetMessageCount")]
    public class Log_ResetMessageCount_EventsCorePatch
    {
        private static bool init;
        public static bool Init => init;

        private static bool Prefix()
        {
            if (!init)
            {
                init = true;

                EventsCoreMod.Settings.CacheSettings();
            }

            return true;
        }
    }
}
