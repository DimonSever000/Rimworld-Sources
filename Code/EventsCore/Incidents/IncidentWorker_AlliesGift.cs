using EventsCore.Utilities;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EventsCore.Incidents
{
    public class IncidentWorker_AlliesGift : IncidentWorker
    {
        private static readonly Predicate<Faction> FactionValidator = x => !x.defeated && x.AllyTo(Faction.OfPlayerSilentFail);
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }

            return Find.FactionManager.AllFactionsListForReading.Any(x => FactionValidator(x));
        }
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!(parms.target is Map map))
            {
                return false;
            }

            if (Find.FactionManager.AllFactionsListForReading.Where(x => FactionValidator(x)).TryRandomElement(out Faction faction))
            {
                List<Thing> things = ThingSetMakerDefOfLocal.AlliesGift.root.Generate();

                if (things.NullOrEmpty())
                {
                    return false;
                }

                IntVec3 intVec = DropCellFinder.RandomDropSpot(map);
                DropPodUtility.DropThingsNear(intVec, map, things, 110, canInstaDropDuringInit: false, leaveSlag: true);

                SendStandardLetter(def.letterLabel, def.letterText, def.letterDef, parms, new TargetInfo(intVec, map), faction);

                return true;
            }

            return false;
        }
    }
}
