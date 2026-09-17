using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;

namespace EventsCore
{
    public class MentalState_HuntingForPlayerPlants : MentalState_TantrumAll
    {
        protected override void GetPotentialTargets(List<Thing> outThings)
        {
            outThings.Clear();
            outThings.AddRange((from x in pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Plant)
                                where ((Plant)x).BlightableNow
                                select x).ToList());
        }
    }
}
