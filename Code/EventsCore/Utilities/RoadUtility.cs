using EventsCore.World;
using RimWorld.Planet;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using UnityEngine.Analytics;
using static HarmonyLib.Code;
using EventsCore.Incidents;
using EventsCore.Settings;

namespace EventsCore.Utilities
{
    [StaticConstructorOnStartup]
    public static class RoadUtility
    {

        public static bool TryPlaceRoadByPoints(RoadDef def, int start, int end, Predicate<WorldPath> passCondition, List<PlanetTile> tiles = null, bool neighborsConnect = true)
        {
            if (TryPlaceRoadForNeighbors(def, start, end, tiles))
            {
                return true;
            }

            WorldPath worldPath = Find.WorldGrid.Surface.Pather.FindPath(start, end, null);

            if (!passCondition.Invoke(worldPath))
            {
                worldPath.ReleaseToPool();
                return false;
            }

            return TryPlaceRoadByPath(def, worldPath, tiles, neighborsConnect);
        }

        public static bool TryPlaceRoadByPath(RoadDef def, WorldPath worldPath, List<PlanetTile> tiles, bool neighborsConnect = true)
        {
            if (!worldPath.Found)
            {
                worldPath.ReleaseToPool();
                return false;
            }

            for (int i = 0; i < worldPath.NodesReversed.Count - 1; i++)
            {
                TryPlaceRoadForNeighbors(def, worldPath.NodesReversed[i], worldPath.NodesReversed[i + 1], tiles);
            }

            if (neighborsConnect)
            {
                TryFindWaterNeighborAndPlaceRoad(def, worldPath.NodesReversed[0], out _);
                TryFindWaterNeighborAndPlaceRoad(def, worldPath.NodesReversed[worldPath.NodesReversed.Count - 1], out _);
            }

            worldPath.ReleaseToPool();

            return true;
        }

        private static bool TryFindWaterNeighborAndPlaceRoad(RoadDef def, PlanetTile point, out PlanetTile selectedNeighbor)
        {
            List<PlanetTile> neighbors = new List<PlanetTile>(6);
            Find.WorldGrid.GetTileNeighbors(point, neighbors);

            if (neighbors.Where(x =>
            {
                if (x.Tile is SurfaceTile tile)
                {
                    return !tile.Roads.NullOrEmpty();
                }

                return false;
            }).TryRandomElement(out selectedNeighbor))
            {
                return TryPlaceRoadForNeighbors(def, point, selectedNeighbor);
            }

            return false;
        }
        public static bool TryPlaceRoadForNeighbors(RoadDef def, PlanetTile start, PlanetTile end, List<PlanetTile> tiles = null)
        {
            if (Find.WorldGrid.IsNeighbor(end, start))
            {
                Find.WorldGrid.OverlayRoad(start, end, def);

                if (tiles != null)
                {
                    tiles.Add(start);
                    tiles.Add(end);
                }

                return true;
            }

            return false;
        }

        public static bool TryFindRoadPointsWithSettlements(out PlanetTile start, out PlanetTile end)
        {
            IncidentWorker_RoadConstruction worker = IncidentDefOfLocal.RoadConstruction.Worker as IncidentWorker_RoadConstruction;

            foreach (Settlement settlement in Find.WorldObjects.Settlements)
            {
                if (!IsValidSettlementForStartRoad(settlement, worker.Props.minTechLevel))
                {
                    continue;
                }

                int maxDist = (int)(worker.Props.roadLengthByTechLevelCurve.Evaluate((int)settlement.Faction.def.techLevel) * EventsCoreMod.Settings.TryGetSettings<WorldSettings>().aiWorldBuildngSize);

                if (TryFindRoadEndPointSettlement(settlement.Tile, settlement.Faction, 3, maxDist, out PlanetTile roadEnd))
                {
                    start = settlement.Tile;
                    end = roadEnd;
                    return true;
                }
            }

            start = -1;
            end = -1;
            return false;
        }

        private static bool IsValidSettlementForStartRoad(Settlement settlement, TechLevel minTechLevel)
        {
            if (settlement.Faction == null || settlement.Faction.IsPlayer)
            {
                return false;
            }

            if (settlement.Faction.def.techLevel < minTechLevel)
            {
                return false;
            }

            Tile tile = Find.WorldGrid[settlement.Tile];
            if (tile is SurfaceTile surfaceTile)
            {
                if (!surfaceTile.PrimaryBiome.allowRoads || !surfaceTile.potentialRoads.NullOrEmpty())
                {
                    return false;
                }
            }
            return true;
        }

        private static bool TryFindRoadEndPointSettlement(PlanetTile start, Faction faction, int minDist, int maxDist, out PlanetTile result)
        {
            if (TileFinder.TryFindPassableTileWithTraversalDistance(start, minDist, maxDist, out result, x =>
            {
                if (x == start)
                {
                    return false;
                }

                Tile tile = Find.WorldGrid[x];
                if (!tile.PrimaryBiome.allowRoads)
                {
                    return false;
                }

                Settlement s = Find.WorldObjects.SettlementAt(x);
                return s != null && !s.Faction.HostileTo(faction);
            }, exitOnFirstTileFound: true))
            {
                return true;
            }

            return false;
        }
    }
}
