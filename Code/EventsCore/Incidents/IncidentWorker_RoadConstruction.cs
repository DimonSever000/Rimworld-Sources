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
using UnityEngine.SocialPlatforms;
using UnityEngine.XR;
using Verse;
using Verse.Noise;
using static System.Collections.Specialized.BitVector32;
using static UnityEngine.UI.GridLayoutGroup;

namespace EventsCore.Incidents
{
    public class IncidentWorker_RoadConstruction : IncidentWorkerWithProps
    {
        public IncidentProperties_RoadConstruction Props => props as IncidentProperties_RoadConstruction;

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }

            if (!RoadUtility.TryFindRoadPointsWithSettlements(out PlanetTile start, out PlanetTile end))
            {
                return false;
            }

            return true;
        }
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!RoadUtility.TryFindRoadPointsWithSettlements(out PlanetTile start, out PlanetTile end))
            {
                return false;
            }

            Settlement from = Find.WorldObjects.SettlementAt(start);
            Settlement to = Find.WorldObjects.SettlementAt(end);

            if (from == null || to == null || from == to)
            {
                Log.Error($"IncidentWorker_RoadConstruction try execute for invalid tiles");
                return false;
            }

            TechLevelRoad techLevelRoad = Props.techLevelRoadMap.Find(x => x.techLevel == from.Faction.def.techLevel);

            if (techLevelRoad == null)
            {
                return false;
            }

            Predicate<int> withoutPlayer = x =>
            {
                Settlement s = Find.WorldObjects.SettlementAt(x);
                return s == null || s.Faction != Faction.OfPlayerSilentFail;
            };

            if (RoadUtility.TryPlaceRoadByPoints(techLevelRoad.roadDef, from.Tile, end, x => x.NodesReversed.All(n => withoutPlayer(n))))
            {
                WorldUtility.ResetWorldRenderer();

                SendStandardLetter(def.letterLabel, def.letterText, def.letterDef, parms, new LookTargets(from, to),
                    from.Faction, from.Name.Colorize(UIUtility.YellowColor), to.Name.Colorize(UIUtility.YellowColor));

                if (DebugSettings.godMode)
                {
                    Find.WorldSelector.SelectedTile = start;
                    Find.WorldCameraDriver.JumpTo(start);
                }

                return true;
            }

            return false;
        }
    }
}
