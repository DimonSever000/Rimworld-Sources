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

namespace EventsCore.Patches
{
    /// <summary>
    /// Этот патч на конструктор WorldDrawLayer_Rivers. Вызывается в момент создания слоя рек, по идее только при загрузках мира.
    /// Используется для изменения цвета рек в мире
    /// </summary>
    [HarmonyPatch(typeof(WorldDrawLayer_Rivers), MethodType.Constructor)]
    public class WorldDrawLayer_Rivers_Constructor_EventsCorePatch
    {
        private static void Postfix(ref WorldDrawLayer_Rivers __instance)
        {
            Color32 color = GameComponent_EventsCore.CurrentGame.PlanetWeatherController.CurPlanetRiversColor();

            AccessTools.Field(typeof(WorldDrawLayer_Rivers), "riverColor").SetValue(__instance, color);
        }
    }
}
