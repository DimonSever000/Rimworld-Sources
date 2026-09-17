using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.AI;
using EventsCore.Utilities;
using static HarmonyLib.Code;
using RimWorld.Planet;

namespace EventsCore.Incidents
{
    public class IncidentWorker_TurtleInvasion : IncidentWorkerWithProps
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }

            if (!(parms.target is Map map))
            {
                return false;
            }

            if (Find.World.CoastDirectionAt(map.Tile) == Rot4.Invalid)
            {
                return false;
            }

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!(parms.target is Map map))
            {
                return false;
            }

            TraverseParms traverseParams = TraverseParms.For(TraverseMode.NoPassClosedDoors).WithFenceblocked(forceFenceblocked: true);

            if (RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith((IntVec3 x) => x.InBounds(map) && x.Standable(map) && x.GetTerrain(map).IsWater && !x.Fogged(map) && map.reachability.CanReachMapEdge(x, traverseParams) && x.GetRoom(map).CellCount >= 225, map, out var result))
            {
                float points = parms.points;

                List<Pawn> list = AggressiveAnimalIncidentUtility.GenerateAnimals(PawnKindDefOfLocal.Tortoise, map.Tile, points);

                if (list.NullOrEmpty())
                {
                    return false;
                }

                for (int i = 0; i < list.Count; i++)
                {
                    IntVec3 loc = CellFinder.RandomSpawnCellForPawnNear(result, map, 10);
                    GenSpawn.Spawn(list[i], loc, map, Rot4.Random);
                    list[i].health.AddHediff(HediffDefOf.Scaria);
                    list[i].mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent);
                }

                SendStandardLetter(def.letterLabel, def.letterText, def.letterDef, parms, list, $"{list.Count}".Colorize(UIUtility.YellowColor));
                return true;
            }

            return false;
        }
    }
}
