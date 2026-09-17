using EventsCore.Settings;
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
using static HarmonyLib.Code;
using static System.Collections.Specialized.BitVector32;
using static UnityEngine.UI.GridLayoutGroup;

namespace EventsCore.Incidents
{
    public class IncidentWorker_IrrigationConstruction : IncidentWorkerWithProps
    {
        private IncidentProperties_IrrigationConstruction Props => props as IncidentProperties_IrrigationConstruction;

        private bool TryFindSettlementWithoutRiver(out Settlement result)
        {
            if (Find.WorldObjects.Settlements.Where(settlement =>
            {
                if (settlement.Faction == null || settlement.Faction.IsPlayer || settlement.Faction.def.techLevel < Props.minTechLevel)
                {
                    return false;
                }

                SurfaceTile tile = Find.WorldGrid.Surface[settlement.Tile];

                if (!tile.PrimaryBiome.allowRivers || !tile.potentialRivers.NullOrEmpty())
                {
                    return false;
                }

                return true;

            }).TryRandomElement(out result))
            {
                return true;
            }

            return false;
        }
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }

            if (!TryFindSettlementWithoutRiver(out Settlement settlement))
            {
                return false;
            }

            return true;
        }
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!TryFindSettlementWithoutRiver(out Settlement settlement))
            {
                return false;
            }

            int maxDist = (int)(Props.riverLengthByTechLevelCurve.Evaluate((int)settlement.Faction.def.techLevel) * EventsCoreMod.Settings.TryGetSettings<WorldSettings>().aiWorldBuildngSize);

            if (RiverUtility.TryFindRiver(settlement.Tile, RiverDefOfLocal.River.widthOnWorld, float.MaxValue, 1, maxDist, out PlanetTile end))
            {
                Predicate<WorldPath> withoutPlayer = x => !x.NodesReversed.Any(t =>
                {
                    foreach (WorldObject worldObject in Find.WorldObjects.ObjectsAt(t))
                    {
                        if (worldObject.Faction == Faction.OfPlayerSilentFail)
                        {
                            return true;
                        }
                    }

                    return false;
                });

                if (RiverUtility.TryPlaceRiverByPoints(RiverDefOfLocal.River, settlement.Tile, end, withoutPlayer))
                {
                    WorldUtility.ResetWorldRenderer();

                    SendStandardLetter(def.letterLabel, def.letterText, def.letterDef, parms, settlement, settlement.Faction, settlement.Name.Colorize(UIUtility.YellowColor));

                    return true;
                }
            }

            return false;
        }
    }
}
