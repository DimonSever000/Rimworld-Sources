using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EventsCore.Incidents
{
    public class IncidentWorker_SeaAir : IncidentWorker_MakeGameCondition
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }

            if (parms.target is Map map)
            {
                if (Find.World.CoastDirectionAt(map.Tile).IsValid)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
