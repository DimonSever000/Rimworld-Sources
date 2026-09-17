using EventsCore.Incidents;
using EventsCore.Settings;
using EventsCore.Utilities;
using HarmonyLib;
using LudeonTK;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Verse;

namespace EventsCore.GameConditions
{
    public class GameCondition_ClimateChange : GameCondition
    {
        private IncidentDef parentIncident;
        public IncidentDef ParentIncident => parentIncident;
        public IncidentProperties_ClimateChange Props => (ParentIncident.Worker as IncidentWorker_ClimateChange).Props;

        public override string Description => base.Description
           .Formatted(
               $"{targetTemperatureChange.ToStringByStyle(ToStringStyle.Temperature)}".Colorize(ColoredText.TipSectionTitleColor),
               $"{curTemperatureChange.ToStringByStyle(ToStringStyle.Temperature)}".Colorize(ColoredText.TipSectionTitleColor),
               TemperatureChangeProgress.ToStringPercent().Colorize(ColoredText.TipSectionTitleColor),
               $"{targetElevationChange}".Colorize(ColoredText.TipSectionTitleColor),
               $"{curElevationChange}".Colorize(ColoredText.TipSectionTitleColor),
               ElevationChangeProgress.ToStringPercent().Colorize(ColoredText.TipSectionTitleColor))
           .Resolve();


        /// <summary>
        /// Частота проверки
        /// </summary>
        private static int CheckInterval => 60000;

        /// <summary>
        /// Когда среднее изменение температуры доходит до этого числа, дальнейшее изменение останавливается
        /// </summary>
        private float targetTemperatureChange = 0f;
        public float TargetTemperatureChange => targetTemperatureChange;

        /// <summary>
        /// Когда среднее изменение высоты доходит до этого числа, дальнейшее изменение останавливается
        /// </summary>
        private float targetElevationChange = 0f;
        public float TargetElevationChange => targetElevationChange;

        /// <summary>
        /// Текущий прогресс от 0 до 1
        /// </summary>
        public float TemperatureChangeProgress
        {
            get
            {
                float totalDelta = Mathf.Abs(targetTemperatureChange);
                float currentDelta = Mathf.Abs(curTemperatureChange);

                if (Mathf.Approximately(totalDelta, 0f))
                {
                    return 1f;
                }

                return Mathf.Clamp01(currentDelta / totalDelta);
            }
        }

        /// <summary>
        /// Текущий показатель изменения температуры по планете
        /// </summary>
        private float curTemperatureChange = 0f;

        /// <summary>
        /// Текущий показатель изменения высоты по планете
        /// </summary>
        private float curElevationChange = 0f;

        /// <summary>
        /// Текущий прогресс от 0 до 1
        /// </summary>
        public float ElevationChangeProgress
        {
            get
            {
                float totalDelta = Mathf.Abs(targetElevationChange);
                float currentDelta = Mathf.Abs(curElevationChange);

                if (Mathf.Approximately(totalDelta, 0f))
                {
                    return 1f;
                }

                return Mathf.Clamp01(currentDelta / totalDelta);
            }
        }

        public override WeatherDef ForcedWeather()
        {
            return def.weatherDef;
        }

        public override void Init()
        {
            base.Init();
            this.Permanent = true;
        }

        /// <summary>
        /// В случае выпадения другого ивента на изменение климата пытаемся совместить целевые показатели изменений с текущим событием
        /// </summary>
        public void Initialize(IncidentDef incidentDef, float? forcedTargetTemperatureChange = null, float? forcedTargetElevationChange = null)
        {
            parentIncident = incidentDef;

            if (forcedTargetTemperatureChange.HasValue)
            {
                targetTemperatureChange += forcedTargetTemperatureChange.Value;
            }
            else
            {
                this.targetTemperatureChange += Props.targetTemperatureChange;
            }

            if (forcedTargetElevationChange.HasValue)
            {
                targetElevationChange += forcedTargetElevationChange.Value;
            }
        }

        public bool ShouldEnd()
        {
            if (TemperatureChangeProgress >= 1f && ElevationChangeProgress >= 1f)
            {
                return true;
            }

            return false;
        }

        public override void GameConditionTick()
        {
            if (Find.TickManager.TicksGame % CheckInterval == 0)
            {
                ChangeWorld();
            }

            if (ShouldEnd())
            {
                this.End();
            }
        }

        public void ChangeWorld()
        {
            ChangeWorldElevation();
            ChangeWorldTemperature();
            ChangeWorldBiomes();
            ClearWorldObjectsIfNeeded();
        }

        private void ClearWorldObjectsIfNeeded()
        {
            for (int i = Find.WorldObjects.AllWorldObjects.Count - 1; i >= 0; i--)
            {
                WorldObject worldObject = Find.WorldObjects.AllWorldObjects[i];

                if (worldObject.Tile.Tile.WaterCovered)
                {
                    Find.WorldObjects.Remove(worldObject);
                }
            }
        }

        private void ChangeWorldElevation()
        {
            if (Mathf.Abs(curElevationChange) >= Mathf.Abs(TargetElevationChange))
            {
                return;
            }

            float elevation = 0f;

            foreach (Tile tile in Find.WorldGrid.Surface.Tiles)
            {
                float elevationChange = ElevationChangeForTile(tile.tile);
                tile.elevation += elevationChange;
                elevation += elevationChange;
            }

            // подсчет изменения высоты по планете
            // в текущей реализации нет множителей в зависимости от тайла, поэтому они все меняются на одну величину
            float delta = elevation / Find.WorldGrid.Surface.TilesCount;
            curElevationChange += delta;
            GameComponent_EventsCore.CurrentGame.PlanetWeatherController.CurGlobalElevationChange += delta;
        }

        private float ElevationChangeForTile(int tileId)
        {
            Tile tile = Find.WorldGrid[tileId];

            float result = TargetElevationChange * Props.elevationChangePerIntervalByDayPassedCurve.Evaluate(GenDate.TicksToDays(TicksPassed));

            return result;
        }

        private void ChangeWorldTemperature()
        {
            if (Mathf.Abs(curTemperatureChange) >= Mathf.Abs(TargetTemperatureChange))
            {
                return;
            }

            float tmp = 0f;

            foreach (Tile tile in Find.WorldGrid.Surface.Tiles)
            {
                float tempChange = TemperatureChangeForTile(tile.tile);
                tile.temperature += tempChange;
                tmp += tempChange;
            }

            // подсчет изменения средней температуры по планете
            float delta = tmp / Find.WorldGrid.Surface.TilesCount;
            curTemperatureChange += delta;
            GameComponent_EventsCore.CurrentGame.PlanetWeatherController.CurGlobalTemperatureChange += delta;
        }

        private float TemperatureChangeForTile(int tileId)
        {
            Tile tile = Find.WorldGrid[tileId];

            float result = TargetTemperatureChange * Props.temperatureChangePerIntervalByDayPassedCurve.Evaluate(GenDate.TicksToDays(TicksPassed));
            result *= Props.elevationTemperatureCurve.Evaluate(tile.elevation);
            result *= Props.equatorialDistanceTemperatureCurve.Evaluate(Find.WorldGrid.Surface.DistanceFromEquatorNormalized(tileId));

            return result;
        }

        private void ChangeWorldBiomes()
        {
            bool recache = false;

            foreach (Tile tile in Find.WorldGrid.Surface.Tiles)
            {
                if (tile.tile.Valid)
                {
                    BiomeDef newDef = WorldUtility.BiomeFrom(tile, tile.tile);
                    if (tile.PrimaryBiome != newDef)
                    {
                        tile.PrimaryBiome = newDef;

                        recache = true;
                    }

                    // Принудительно удаляем все landmarks и Mutators сразу, потому что они вызывают ошибки при переменах биомов
                    Find.World.landmarks?.RemoveLandmark(tile.tile);
                    for (int i = tile.Mutators.Count - 1; i >= 0; i--)
                    {
                        tile.RemoveMutator(tile.Mutators[i]);
                    }
                }
            }

            if (recache)
            {
                WorldUtility.ResetWorldRenderer();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref curTemperatureChange, "curTemperatureChange");
            Scribe_Values.Look(ref targetTemperatureChange, "targetTemperatureChange");
            Scribe_Values.Look(ref curElevationChange, "curElevationChange");
            Scribe_Values.Look(ref targetElevationChange, "targetElevationChange");
            Scribe_Defs.Look(ref parentIncident, "parentIncident");
        }
    }
}
