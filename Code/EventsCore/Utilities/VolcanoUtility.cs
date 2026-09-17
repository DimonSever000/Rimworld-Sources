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
using UnityEngine.SocialPlatforms;
using EventsCore.Incidents;
using EventsCore.Settings;


namespace EventsCore.Utilities
{
    [StaticConstructorOnStartup]
    public static class VolcanoUtility
    {
        private static readonly SimpleCurve lavaSolidificationByDepthCurve = new SimpleCurve()
        {
            new CurvePoint(0, 3),
            new CurvePoint(30, 10),
        };

        private static List<PlanetTile> tmpTiles = new List<PlanetTile>();

        private static List<PlanetTile> volcanos = new List<PlanetTile>();

        public static bool TryGetVolcanos(out List<PlanetTile> outList)
        {
            outList = null;

            if (!ModsConfig.OdysseyActive)
            {
                return false;
            }

            outList = null;
            RecacheVolcanos();

            if (volcanos.Any())
            {
                outList = volcanos.ToList();
                return true;
            }

            return false;
        }

        private static bool IsVolcano(int tileID)
        {
            Tile tile = Find.WorldGrid[tileID];

            if (tile.Landmark != null && tile.Landmark.def == LandmarkDefOfLocal.LavaCrater)
            {
                return true;
            }

            return false;
        }
        private static void RecacheVolcanos()
        {
            volcanos.Clear();

            for (int tileID = 0; tileID < Find.WorldGrid.Surface.TilesCount; tileID++)
            {
                if (IsVolcano(tileID))
                {
                    volcanos.Add(tileID);
                }
            }
        }
        public static bool TrySupervolcanoExplosion(out int center, float power = 1f)
        {
            center = -1;

            if (!ModsConfig.OdysseyActive)
            {
                return false;
            }

            RecacheVolcanos();

            if (volcanos.TryRandomElement(out PlanetTile tileID))
            {
                return TrySupervolcanoExplosion(tileID, out center, power);
            }

            return false;
        }
        public static bool TrySupervolcanoExplosion(int tileID, out int center, float power = 1f)
        {
            center = -1;

            if (!ModsConfig.OdysseyActive)
            {
                return false;
            }

            if (!IsVolcano(tileID))
            {
                return false;
            }

            Tile rootTile = Find.WorldGrid.Surface[tileID];

            center = tileID;

            float volcanoPower = rootTile.elevation * 3f * power * EventsCoreMod.Settings.TryGetSettings<WorldSettings>().volcanoesEruptionFactor;
            int i = 0;

            RecursiveLavaFlow(tileID, ref volcanoPower, 0, ref i);
            MakeAsh(tileID, power);

            WorldUtility.ResetWorldRenderer();
            return true;
        }

        private static void MakeAsh(int tileID, float power)
        {
            IncidentWorker_SupervolcanoExplosion worker = IncidentDefOfLocal.SupervolcanoExplosion.Worker as IncidentWorker_SupervolcanoExplosion;
            var props = worker.Props;

            int ashCount = (int)props.ashByPowerCurve.Evaluate(power);
            int range = props.explodeRadius.RandomInRange;

            for (int i = 0; i < ashCount; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    if (TileFinder.TryFindPassableTileWithTraversalDistance(tileID, range, range + 10, out PlanetTile volcanicAshTargetTileID, ignoreFirstTilePassability: true, canTraverseImpassable: true) && !tmpTiles.Contains(volcanicAshTargetTileID))
                    {
                        WorldObject_VolcanicAsh volcanicAsh = (WorldObject_VolcanicAsh)WorldObjectMaker.MakeWorldObject(WorldObjectDefOfLocal.VolcanicAsh);
                        volcanicAsh.Tile = tileID;
                        volcanicAsh.lifeSpan = 60000 * Rand.Range(10, 40);
                        volcanicAsh.period = (int)(20000f * Rand.Range(1.2f, 2f));

                        volcanicAsh.destinationTile = volcanicAshTargetTileID;

                        Find.WorldObjects.Add(volcanicAsh);

                        tmpTiles.Add(volcanicAshTargetTileID);

                        break;
                    }
                }
            }
        }

        private static void RecursiveLavaFlow(int curRoot, ref float volcanoPower, int curDepth, ref int i)
        {
            if (!ModsConfig.OdysseyActive)
            {
                return;
            }

            if (i > 500)
            {
                return;
            }
            i++;

            SurfaceTile curTile = Find.WorldGrid.Surface[curRoot];
            
            if (curTile.PrimaryBiome != BiomeDefOfLocal.LavaField)
            {
                if (curTile.elevation < 0)
                {
                    curTile.elevation = 0;
                }

                curTile.elevation += Rand.Range(25f, 40f);

                curTile.PrimaryBiome = BiomeDefOfLocal.LavaField;
            }

            curTile.potentialRoads?.Clear();
            curTile.swampiness = 0f;

            foreach (WorldObject worldObject in Find.WorldObjects.AllWorldObjects.ToList())
            {
                if (worldObject.Tile == curRoot)
                {
                    Find.WorldObjects.Remove(worldObject);
                }
            }

            Find.WorldGrid.GetTileNeighbors(curRoot, tmpTiles);
            float power = volcanoPower;

            if (tmpTiles.Where(x => Find.WorldGrid[x].elevation <= power && Find.WorldGrid[x].PrimaryBiome != BiomeDefOfLocal.LavaField).TryRandomElement(out PlanetTile nextTileID))
            {
                Tile nextTile = Find.WorldGrid[nextTileID];
                volcanoPower -= Mathf.Abs(nextTile.elevation);

                RecursiveLavaFlow(nextTileID, ref volcanoPower, curDepth + 1, ref i);
            }
        }
    }
}
