using EventsCore.GameConditions;
using EventsCore.Incidents;
using EventsCore.Utilities;
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
using Verse;

namespace EventsCore.Utilities
{
    public static class DebugTools
    {
        public const string DebugActionCategory = "Events+";

        [DebugAction(DebugActionCategory, "Climate Change Step", false, false, false, false, false, 0, false, allowedGameStates = AllowedGameStates.PlayingOnMap, displayPriority = 1000)]
        private static void ChangeWorldBiomesAndTemperature()
        {
            GameCondition_ClimateChange condition = Find.World.GameConditionManager.GetActiveCondition<GameCondition_ClimateChange>();
            if (condition != null)
            {
                condition.ChangeWorld();
            }
        }

        [DebugAction(DebugActionCategory, "Print Planet Weather Controller Report", false, false, false, false, false, 0, false, allowedGameStates = AllowedGameStates.PlayingOnMap, displayPriority = 1000)]
        private static void PrintPlanetWeatherControllerReport()
        {
            GameComponent_EventsCore.CurrentGame.PlanetWeatherController.PrintReport();
        }

        [DebugAction(DebugActionCategory, "Try Build Artificial Channel", false, false, false, false, false, 0, false, allowedGameStates = AllowedGameStates.WorldRenderedNow, displayPriority = 1000)]
        private static void TryBuildArtificialChannel()
        {
            if (RiverUtility.TryBuildArtificialChannel(out PlanetTile start, out PlanetTile end, out _))
            {
                Find.WorldSelector.SelectedTile = start;
                Find.WorldCameraDriver.JumpTo(start);
            }
        }

        [DebugAction(DebugActionCategory, "Try Execute Irrigation Construction", false, false, false, false, false, 0, false, allowedGameStates = AllowedGameStates.WorldRenderedNow, displayPriority = 1000)]
        private static void TryExecuteIrrigationConstruction()
        {
            IncidentDefOfLocal.IrrigationConstruction.Worker.TryExecute(StorytellerUtility.DefaultParmsNow(IncidentDefOfLocal.IrrigationConstruction.category, Find.AnyPlayerHomeMap));
        }

        [DebugAction(DebugActionCategory, "Try Execute Road Construction", false, false, false, false, false, 0, false, allowedGameStates = AllowedGameStates.WorldRenderedNow, displayPriority = 1000)]
        private static void TryExecuteRoadConstruction()
        {
            IncidentDefOfLocal.RoadConstruction.Worker.TryExecute(StorytellerUtility.DefaultParmsNow(IncidentDefOfLocal.RoadConstruction.category, Find.AnyPlayerHomeMap));
        }

        [DebugAction(DebugActionCategory, "Try Supervolcano Explosion", false, false, false, false, false, 0, false, allowedGameStates = AllowedGameStates.WorldRenderedNow, displayPriority = 1000)]
        private static void TrySupervolcanoExplosion()
        {
            IncidentDefOfLocal.SupervolcanoExplosion.Worker.TryExecute(StorytellerUtility.DefaultParmsNow(IncidentDefOfLocal.SupervolcanoExplosion.category, Find.AnyPlayerHomeMap));
        }

        [DebugAction(DebugActionCategory, "Try Supervolcano Explosion Select", false, false, false, false, false, 0, false, allowedGameStates = AllowedGameStates.WorldRenderedNow, displayPriority = 1000)]
        private static void TrySupervolcanoExplosionSelect()
        {
            if (VolcanoUtility.TryGetVolcanos(out List<PlanetTile> volcanos))
            {
                List<DebugMenuOption> list = new List<DebugMenuOption>();

                foreach (int tile in volcanos)
                {
                    list.Add(new DebugMenuOption($"{tile}", DebugMenuOptionMode.Action, delegate
                    {
                        VolcanoUtility.TrySupervolcanoExplosion(tile, out int center, 8f);
                    }));
                }

                Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
            }
        }

        [DebugAction(DebugActionCategory, "Try Execute Global Cooling", false, false, false, false, false, 0, false, allowedGameStates = AllowedGameStates.WorldRenderedNow, displayPriority = 1000)]
        private static void TryExecuteGlobalCooling()
        {
            IncidentDefOfLocal.GlobalCooling.Worker.TryExecute(StorytellerUtility.DefaultParmsNow(IncidentDefOfLocal.GlobalCooling.category, Find.World));
        }

        [DebugAction(DebugActionCategory, "Try Execute Global Warming", false, false, false, false, false, 0, false, allowedGameStates = AllowedGameStates.WorldRenderedNow, displayPriority = 1000)]
        private static void TryExecuteGlobalWarming()
        {
            IncidentDefOfLocal.GlobalWarming.Worker.TryExecute(StorytellerUtility.DefaultParmsNow(IncidentDefOfLocal.GlobalWarming.category, Find.World));
        }
    }
}
