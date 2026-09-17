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
using UnityEngine.Tilemaps;
using UnityEngine.XR;
using Verse;
using Verse.Noise;
using static UnityEngine.UI.GridLayoutGroup;

namespace EventsCore.Incidents
{
    public class IncidentWorker_ArtificialChannelConstruction : IncidentWorkerWithProps
    {
        private IncidentProperties_ArtificialChannelConstruction Props => props as IncidentProperties_ArtificialChannelConstruction;

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }

            if (!RiverUtility.TryFindArtificialChannelStartPoint(out PlanetTile start))
            {
                return false;
            }

            if (!Find.FactionManager.TryGetRandomNonColonyHumanlikeFaction(out Faction faction, true, false, Props.minTechLevel))
            {
                return false;
            }

            float mult = Props.channelLengthByTechLevelCurve.Evaluate((int)faction.def.techLevel);
            int min = (int)(Props.defaultChannelLength.TrueMin * mult);
            int max = (int)(Props.defaultChannelLength.TrueMax * mult);

            if (!RiverUtility.TryFindArtificialChannelEndPoint(start, min, max, out PlanetTile end))
            {
                return false;
            }

            return true;
        }
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!Find.FactionManager.TryGetRandomNonColonyHumanlikeFaction(out Faction faction, true, false, Props.minTechLevel))
            {
                return false;
            }

            float mult = Props.channelLengthByTechLevelCurve.Evaluate((int)faction.def.techLevel);
            mult *= EventsCoreMod.Settings.TryGetSettings<WorldSettings>().aiWorldBuildngSize;
            int min = (int)(Props.defaultChannelLength.TrueMin * mult);
            int max = (int)(Props.defaultChannelLength.TrueMax * mult);

            if (RiverUtility.TryBuildArtificialChannel(out PlanetTile start, out PlanetTile end, out WorldFeature worldFeature, min, max, true, false))
            {
                WorldUtility.ResetWorldRenderer();

                SendStandardLetter(def.letterLabel, def.letterText, def.letterDef, parms, new LookTargets(start), faction, worldFeature.name.Colorize(UIUtility.YellowColor));

                return true;
            }

            return false;
        }
    }
}
