using EventsCore.GenSteps;
using EventsCore.Utilities;
using EventsCore.World;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.XR;
using Verse;
using Verse.Noise;
using static UnityEngine.UI.GridLayoutGroup;

namespace EventsCore.Incidents
{
    public class IncidentWorker_WorldMeteorite : IncidentWorker
    {
        private static List<int> cachedTilesTmp = new List<int>();
        public static int MeteoriteSize => 3;

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            List<ThingDef> mineables = ((GenStep_WorldMeteorite)GenStepDefOfLocal.WorldMeteorite.genStep).mineables;

            if (mineables.TryRandomElementByWeight(x => x.building.mineableScatterCommonality, out ThingDef mineable))
            {
                if (TileFinder.TryFindNewSiteTile(out PlanetTile tile, 10, 99999, true, tileFinderMode: TileFinderMode.Random))
                {
                    Slate slate = new Slate();
                    slate.Set("targetMineable", mineable);
                    slate.Set("targetMineableThing", mineable.building.mineableThing);
                    slate.Set($"siteTile", tile);

                    if (QuestScriptDefOfLocal.WorldMeteorite.CanRun(slate, parms.target))
                    {
                        Quest quest = QuestUtility.GenerateQuestAndMakeAvailable(QuestScriptDefOfLocal.WorldMeteorite, slate);
                        Find.LetterStack.ReceiveLetter(quest.name, quest.description, LetterDefOf.PositiveEvent, null, null, quest);

                        MeteoriteWorldLanding(tile);

                        return true;
                    }
                }
            }

            return false;
        }

        private void DoFloodFill(int center, int range)
        {
            Find.WorldGrid.Surface.Filler.FloodFill(center, (PlanetTile tile) => true, delegate (PlanetTile tile, int dist)
            {
                if (dist > range)
                {
                    return true;
                }

                if (dist == range)
                {
                    cachedTilesTmp.Add(tile);
                }

                return false;
            });
        }

        private void MeteoriteWorldLanding(int center)
        {
            Tile tile = Find.World.grid[center];
            tile.elevation = Mathf.Max(1f, tile.elevation - Rand.Range(300f, 500f));
            tile.hilliness = Hilliness.Flat;
            tile.PrimaryBiome = WorldUtility.BiomeFrom(tile, center);

            PlaceMeteoriteImpactZone(center);
            PlaceMeteoriteImpactWall(center);
            PlaceMeteoriteInertialWall(center);

            WorldUtility.ResetWorldRenderer();

            cachedTilesTmp.Clear();
        }
        private void PlaceMeteoriteImpactZone(int center)
        {
            cachedTilesTmp.Clear();

            DoFloodFill(center, MeteoriteSize - 2);

            for (int i = 0; i < cachedTilesTmp.Count; i++)
            {
                int tileID = cachedTilesTmp[i];
                Tile tile = Find.World.grid[tileID];

                tile.hilliness = Hilliness.Flat;
                tile.elevation -= Rand.Range(75f, 200f);

                if (Find.WorldObjects.AnyWorldObjectAt(tileID))
                {
                    tile.elevation = Mathf.Max(1, tile.elevation);
                }

                tile.PrimaryBiome = WorldUtility.BiomeFrom(tile, tileID);
            }
        }
        private void PlaceMeteoriteImpactWall(int center)
        {
            cachedTilesTmp.Clear();

            DoFloodFill(center, MeteoriteSize - 1);

            for (int i = 0; i < cachedTilesTmp.Count; i++)
            {
                int tileID = cachedTilesTmp[i];
                Tile tile = Find.World.grid[tileID];

                tile.elevation += Rand.Range(400f, 600f);
                tile.PrimaryBiome = WorldUtility.BiomeFrom(tile, tileID);

                if (!tile.WaterCovered)
                {
                    tile.hilliness = Hilliness.Impassable;

                    if (Rand.Chance(0.15f) || Find.WorldObjects.AnyWorldObjectAt(tileID))
                    {
                        tile.hilliness = Hilliness.Mountainous;
                    }
                }
            }
        }

        private void PlaceMeteoriteInertialWall(int center)
        {
            cachedTilesTmp.Clear();

            DoFloodFill(center, MeteoriteSize);

            for (int i = 0; i < cachedTilesTmp.Count; i++)
            {
                int tileID = cachedTilesTmp[i];
                Tile tile = Find.World.grid[tileID];

                tile.elevation = Mathf.Max(1f, Find.World.grid[center].elevation + Rand.Range(200f, 400f));
                tile.PrimaryBiome = WorldUtility.BiomeFrom(tile, tileID);

                if (!tile.WaterCovered)
                {
                    tile.hilliness = Hilliness.SmallHills;
                    if (Rand.Chance(0.33f))
                    {
                        tile.hilliness = Hilliness.LargeHills;
                    }
                }
            }
        }
    }
}
