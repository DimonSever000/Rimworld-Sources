using EventsCore.World;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Analytics;
using Verse;

namespace EventsCore.Utilities
{
    [StaticConstructorOnStartup]
    public static class MiscUtility
    {
        private static float curSunLuminosity = 1f;
        public static float CurSunLuminosity => curSunLuminosity;

        public static float RecalculateCurSunLuminosity()
        {
            float value = 1f;

            if (Find.World?.GameConditionManager != null)
            {
                foreach (GameCondition condition in Find.World.GameConditionManager.ActiveConditions)
                {
                    if (condition is ISunLuminosityInfluencer planetSunLightInfluencer)
                    {
                        value *= planetSunLightInfluencer.SunLuminosityFactor();
                    }
                }
            }

            return value;
        }
        public static void Notify_SunLuminosityChanged()
        {
            curSunLuminosity = RecalculateCurSunLuminosity();

            foreach (WorldDrawLayerBase allDrawLayer in Find.World.renderer.AllDrawLayers)
            {
                if (allDrawLayer is GlobalDrawLayer_Sun globalDrawLayer_Sun)
                {
                    globalDrawLayer_Sun.SetDirty();
                }
            }
        }

    }
}
