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
using HarmonyLib;
using Verse.Profile;

namespace EventsCore.Utilities
{
    [StaticConstructorOnStartup]
    public static class WorldUtility
    {
        public static void ResetWorldRenderer()
        {
            Find.WorldGrid.Surface.InitializeLayer();
        }

        public static BiomeDef BiomeFrom(Tile ws, int tileID)
        {
            List<BiomeDef> allDefsListForReading = DefDatabase<BiomeDef>.AllDefsListForReading;
            BiomeDef biomeDef = null;
            float num = 0f;
            for (int i = 0; i < allDefsListForReading.Count; i++)
            {
                BiomeDef biomeDef2 = allDefsListForReading[i];
                if (biomeDef2.implemented && biomeDef2.generatesNaturally)
                {
                    float score = biomeDef2.Worker.GetScore(biomeDef2, ws, tileID);
                    if (score > num || biomeDef == null)
                    {
                        biomeDef = biomeDef2;
                        num = score;
                    }
                }
            }
            return biomeDef;
        }

        public static bool TryFindWorldFeature(out WorldFeature waterFeature, Predicate<WorldFeature> passCheck)
        {
            if (Find.WorldFeatures.features.Where(x => passCheck(x)).TryRandomElement(out waterFeature))
            {
                return true;
            }

            return false;
        }
        public static bool TryFindWaterFeature(out WorldFeature waterFeature)
        {
            if (TryFindWorldFeature(out waterFeature,
                x => x.def == FeatureDefOfLocal.Lake ||
                x.def == FeatureDefOfLocal.Sea ||
                x.def == FeatureDefOfLocal.Ocean ||
                x.def == FeatureDefOfLocal.OceanBay ||
                x.def == FeatureDefOfLocal.Bay))
            {
                return true;
            }

            return false;
        }

        public static bool TryFindWaterFeatureEdgePoint(out PlanetTile tile)
        {
            tile = -1;
            List<PlanetTile> neighbors = new List<PlanetTile>();

            if (TryFindWaterFeature(out WorldFeature waterFeature))
            {
                // поиск среди береговых тайлов
                if (waterFeature.Tiles.Where(tileId =>
                {
                    Find.WorldGrid.GetTileNeighbors(tileId, neighbors);

                    return neighbors.Any(x => !Find.World.grid[x].WaterCovered);

                }).TryRandomElement(out int randWaterTile))
                {
                    Find.WorldGrid.GetTileNeighbors(randWaterTile, neighbors);

                    if (neighbors.Where(x => !Find.World.grid[x].WaterCovered).TryRandomElement(out PlanetTile randCoastTile))
                    {
                        tile = randCoastTile;

                        return true;
                    }
                }
            }

            return false;
        }
        public static bool TryFindWaterTileNear(PlanetTile tile, out PlanetTile waterTileNear)
        {
            List<PlanetTile> neighbors = new List<PlanetTile>(6);
            Find.WorldGrid.GetTileNeighbors(tile, neighbors);

            if (neighbors.Where(x => Find.World.grid[x].WaterCovered).TryRandomElement(out waterTileNear))
            {
                return true;
            }

            return false;
        }

    }
}
