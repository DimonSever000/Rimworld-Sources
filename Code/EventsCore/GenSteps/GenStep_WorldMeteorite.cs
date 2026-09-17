using EventsCore.Settings;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace EventsCore.GenSteps
{
    public class GenStep_WorldMeteorite : GenStep_ScatterLumpsMineable
    {
        public List<ThingDef> mineables;

        public FloatRange totalValueRange = new FloatRange(1000f, 2000f);
        public override int SeedPart => 1634184422;

        private float curOrePercent;

        public override void Generate(Map map, GenStepParams parms)
        {
            if (parms.sitePart != null && parms.sitePart.parms.preciousLumpResources != null)
            {
                forcedDefToScatter = parms.sitePart.parms.preciousLumpResources;
            }
            else
            {
                forcedDefToScatter = mineables.RandomElement();
            }

            count = 1;
            curOrePercent = EventsCoreMod.Settings.TryGetSettings<WorldSettings>().meteoriteOrePercent;

            float randomInRange = totalValueRange.RandomInRange / curOrePercent;
            float baseMarketValue = forcedDefToScatter.building.mineableThing.BaseMarketValue;

            int forcedLumpSizeInt = Mathf.Max(Mathf.RoundToInt(randomInRange / ((float)forcedDefToScatter.building.mineableYield * baseMarketValue)), 1);
            forcedLumpSize = Mathf.Min(forcedLumpSizeInt, 2000);

            base.Generate(map, parms);
        }

        protected override bool CanScatterAt(IntVec3 c, Map map)
        {
            if (MapGenerator.TryGetVar<List<CellRect>>("UsedRects", out var var) && var.Any((CellRect x) => x.Contains(c)))
            {
                return false;
            }

            return map.reachability.CanReachMapEdge(c, TraverseParms.For(TraverseMode.PassDoors));
        }

        protected override void ScatterAt(IntVec3 c, Map map, GenStepParams parms, int stackCount = 1)
        {
            ThingDef forcedMineral = ChooseThingDef();
            ThingDef rock = DefDatabase<ThingDef>.AllDefs.Where((ThingDef d) => d.IsNonResourceNaturalRock).RandomElement();
            if (forcedMineral == null || rock == null)
            {
                return;
            }

            int numCells = ((forcedLumpSize > 0) ? forcedLumpSize : forcedMineral.building.mineableScatterLumpSizeRange.RandomInRange);
            recentLumpCells.Clear();

            foreach (IntVec3 item in GridShapeMaker.IrregularLump(c, map, numCells))
            {
                if (Current.ProgramState != ProgramState.MapInitializing || !(MapGenerator.Caves[item] > 0f))
                {
                    ThingDef def = forcedMineral;

                    if (Rand.Chance(1f - curOrePercent))
                    {
                        def = rock;
                    }

                    GenSpawn.Spawn(def, item, map);
                    recentLumpCells.Add(item);
                }
            }


            int minX = recentLumpCells.Min((IntVec3 x) => x.x);
            int minZ = recentLumpCells.Min((IntVec3 x) => x.z);
            int maxX = recentLumpCells.Max((IntVec3 x) => x.x);
            int maxZ = recentLumpCells.Max((IntVec3 x) => x.z);
            CellRect rect = CellRect.FromLimits(minX, minZ, maxX, maxZ);
            MapGenerator.SetVar("RectOfInterest", rect);
        }
    }
}
