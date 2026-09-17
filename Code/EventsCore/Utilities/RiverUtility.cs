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

namespace EventsCore.Utilities
{
    [StaticConstructorOnStartup]
    public static class RiverUtility
    {

        public static bool TryBuildArtificialChannel(out PlanetTile start, out PlanetTile end, out WorldFeature worldFeature, int minDist = 5, int maxDist = 50, bool named = true, bool allowLayering = true, int maxTries = 100)
        {
            start = -1;
            end = -1;
            worldFeature = null;

            for (int i = 0; i < maxTries; i++)
            {
                if (!TryFindArtificialChannelStartPoint(out start))
                {
                    continue;
                }

                if (!TryFindArtificialChannelEndPoint(start, minDist, maxDist, out end))
                {
                    continue;
                }

                if (TryBuildArtificialChannelForPoints(start, end, out worldFeature, named, allowLayering))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TryBuildArtificialChannelForPoints(PlanetTile start, PlanetTile end, out WorldFeature worldFeature, bool named = true, bool allowLayering = true)
        {
            worldFeature = null;

            List<PlanetTile> path = new List<PlanetTile>();

            Predicate<WorldPath> withoutWorldObjects = x => !x.NodesReversed.Any(y => Find.WorldObjects.AnyWorldObjectAt(y));
            Predicate<WorldPath> withoutChannels = x => allowLayering || !x.NodesReversed.Any(y =>
            {
                if (y.Tile is SurfaceTile tile)
                {
                    return tile.Roads != null && tile.Roads.Any(r => r.road == RoadDefOfLocal.ArtificialChannelHighway);
                }
                return false;
            });
            Predicate<WorldPath> withoutWorldObjectsAndChannels = x => withoutWorldObjects(x) && withoutChannels(x);

            // прокладываение основного канала
            if (RoadUtility.TryPlaceRoadByPoints(RoadDefOfLocal.ArtificialChannelHighway, start, end, withoutWorldObjectsAndChannels, path))
            {
                PlanetTile neighbor = -1;

                // соединение с морем рядом
                if (WorldUtility.TryFindWaterTileNear(start, out neighbor))
                {
                    RoadUtility.TryPlaceRoadByPoints(RoadDefOfLocal.ArtificialChannelHighway, start, neighbor, withoutWorldObjects, path);
                }
                if (WorldUtility.TryFindWaterTileNear(end, out neighbor))
                {
                    RoadUtility.TryPlaceRoadByPoints(RoadDefOfLocal.ArtificialChannelHighway, end, neighbor, withoutWorldObjects, path);
                }

                if (named)
                {
                    FeatureWorker_Special worker = FeatureDefOfLocal.ArtificialChannel.Worker as FeatureWorker_Special;
                    worker.AddFeatureDirect(Find.WorldGrid.Surface, path, path);
                    Find.WorldFeatures.textsCreated = false;
                    worldFeature = Find.WorldFeatures.features.LastOrDefault();
                }

                WorldUtility.ResetWorldRenderer();

                return true;
            }

            return false;
        }

        public static bool TryFindArtificialChannelStartPoint(out PlanetTile tile)
        {
            if (WorldUtility.TryFindWaterFeatureEdgePoint(out tile))
            {
                return true;
            }

            return false;
        }

        public static bool TryFindArtificialChannelEndPoint(PlanetTile center, int minDist, int maxDist, out PlanetTile point)
        {
            if (TryFindRiver(center, 0.8f, 0.8f, minDist, maxDist, out point))
            {
                return true;
            }

            return false;
        }

        public static bool TryFindRiver(PlanetTile center, float widthMin, float widthMax, int minDist, int maxDist, out PlanetTile outTile)
        {
            int result = -1;
            Find.WorldGrid.Surface.Filler.FloodFill(center, (PlanetTile tile) => true, delegate (PlanetTile tile, int dist)
            {
                Tile t = Find.World.grid[tile];
                if (t is SurfaceTile surfaceTile)
                {
                    if (dist >= minDist && dist <= maxDist)
                    {
                        if (!t.WaterCovered && surfaceTile.Rivers != null && surfaceTile.Rivers.Any(x => x.river.widthOnWorld >= widthMin && x.river.widthOnWorld <= widthMin))
                        {
                            result = tile;
                            return true;
                        }
                    }
                }

                return false;
            });

            outTile = result;

            return outTile != -1;
        }

        public static bool TryPlaceRiverByPoints(RiverDef def, PlanetTile start, PlanetTile end, Predicate<WorldPath> passCondition, List<PlanetTile> tiles = null, bool neighborsConnect = true)
        {
            if (TryPlaceRiverForNeighbors(def, start, end, tiles))
            {
                return true;
            }

            WorldPath worldPath = Find.WorldGrid.Surface.Pather.FindPath(start, end, null);

            if (!passCondition.Invoke(worldPath))
            {
                worldPath.ReleaseToPool();
                return false;
            }

            return TryPlaceRiverByPath(def, worldPath, tiles, neighborsConnect);
        }
        public static bool TryPlaceRiverByPath(RiverDef def, WorldPath worldPath, List<PlanetTile> tiles, bool neighborsConnect = true)
        {
            if (!worldPath.Found)
            {
                worldPath.ReleaseToPool();
                return false;
            }

            for (int i = 0; i < worldPath.NodesReversed.Count - 1; i++)
            {
                TryPlaceRiverForNeighbors(def, worldPath.NodesReversed[i], worldPath.NodesReversed[i + 1], tiles);
            }

            if (neighborsConnect)
            {
                TryFindWaterNeighborAndPlaceRiver(def, worldPath.NodesReversed[0], out _);
                TryFindWaterNeighborAndPlaceRiver(def, worldPath.NodesReversed[worldPath.NodesReversed.Count - 1], out _);
            }

            worldPath.ReleaseToPool();

            return true;
        }
        private static bool TryFindWaterNeighborAndPlaceRiver(RiverDef def, PlanetTile point, out PlanetTile selectedNeighbor)
        {
            List<PlanetTile> neighbors = new List<PlanetTile>(6);
            Find.WorldGrid.Surface.GetTileNeighbors(point, neighbors);

            if (neighbors.Where(x =>
            {
                if (x.Tile is SurfaceTile surfaceTile)
                {
                    return Find.WorldGrid[x].WaterCovered || !surfaceTile.Rivers.NullOrEmpty();
                }
                return false;
            }).TryRandomElement(out selectedNeighbor))
            {
                return TryPlaceRiverForNeighbors(def, point, selectedNeighbor);
            }
            
            return false;
        }

        public static bool TryPlaceRiverForNeighbors(RiverDef def, PlanetTile start, PlanetTile end, List<PlanetTile> tiles = null)
        {
            if (Find.WorldGrid.IsNeighbor(end, start))
            {
                Find.WorldGrid.OverlayRiver(start, end, def);

                if (tiles != null)
                {
                    tiles.Add(start);
                    tiles.Add(end);
                }

                return true;
            }

            return false;
        }
    }
}
