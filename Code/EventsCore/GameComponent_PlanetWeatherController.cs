using EventsCore.Settings;
using EventsCore.Utilities;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace EventsCore
{
    public class GameComponent_PlanetWeatherController : GameComponent
    {
        private static readonly Color32 initialRiverColor = new Color32(73, 82, 100, byte.MaxValue);
        private static readonly Color32 iceRiverColor = new Color32(150, 150, 150, 255);
        private static readonly Color32 parchedRiverColor = new Color32(initialRiverColor.r, initialRiverColor.g, initialRiverColor.b, 0);

        private Game game;
        public Game Game => game;

        private float curGlobalTemperatureChange = 0f;
        public float CurGlobalTemperatureChange { get { return curGlobalTemperatureChange; } set { curGlobalTemperatureChange = value; } }

        private float curGlobalElevationChange = 0f;
        public float CurGlobalElevationChange { get { return curGlobalElevationChange; } set { curGlobalElevationChange = value; } }

        public GameComponent_PlanetWeatherController()
        {

        }

        public GameComponent_PlanetWeatherController(Game game)
        {
            this.game = game;
        }

        public Color32 CurPlanetRiversColor()
        {
            Color32 result = initialRiverColor;
            float absTempChange = Mathf.Abs(CurGlobalTemperatureChange);

            if (CurGlobalTemperatureChange > 0f)
            {
                result = Color.Lerp(initialRiverColor, parchedRiverColor, Mathf.Clamp01(absTempChange / 120f));
            }
            else
            {
                result = Color.Lerp(initialRiverColor, iceRiverColor, Mathf.Clamp01(absTempChange / 120f));
            }

            return result;
        }

        public void PrintReport()
        {
            StringBuilder stringBuilder = new StringBuilder($"Planet Weather Controller Report");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"CurGlobalTemperatureChange = {CurGlobalTemperatureChange}");
            stringBuilder.AppendLine($"CurGlobalElevationChange = {CurGlobalElevationChange}");

            Log.Warning($"{stringBuilder}");
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref curGlobalTemperatureChange, "curGlobalTemperatureChange");
            Scribe_Values.Look(ref curGlobalElevationChange, "curGlobalElevationChange");
        }
    }
}
