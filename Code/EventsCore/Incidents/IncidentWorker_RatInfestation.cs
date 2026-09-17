using EventsCore.Utilities;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using Verse;
using Verse.Noise;

namespace EventsCore.Incidents
{
    public class IncidentWorker_RatInfestation : IncidentWorkerWithProps
    {
        public IncidentProperties_RatInfestation Props => props as IncidentProperties_RatInfestation;

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

            if (TryFindRandomBlightablePlant((Map)parms.target, out _))
            {
                if (!map.mapTemperature.SeasonAndOutdoorTemperatureAcceptableFor(PawnKindDefOfLocal.Rat.race))
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!(parms.target is Map map))
            {
                return false;
            }

            if (!TryFindRandomBlightablePlant(map, out Plant plant))
            {
                return false;
            }

            float range = Props.rangeByPointsCurve.Evaluate(parms.points);
            Room room = plant.GetRoom();

            int ratsCount = Mathf.Max(1, (int)Props.ratByPointCurve.Evaluate(parms.points));
            int curRatsCount = 0;

            foreach (IntVec3 cell in GenRadial.RadialCellsAround(plant.Position, range, true))
            {
                if (curRatsCount >= ratsCount)
                {
                    break;
                }

                if (!cell.InBounds(map) || cell.GetRoom(map) != room)
                {
                    continue;
                }

                Plant firstBlightableNowPlant = BlightUtility.GetFirstBlightableNowPlant(cell, map);
                if (firstBlightableNowPlant != null && firstBlightableNowPlant.def == plant.def && Rand.Chance(0.25f))
                {
                    IntVec3 pos = firstBlightableNowPlant.Position;
                    Pawn pawn = PawnGenerator.GeneratePawn(PawnKindDefOfLocal.Rat);
                    GenSpawn.Spawn(pawn, pos, map);
                    pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOfLocal.HuntingForPlayerPlants, null, forceWake: true);
                    curRatsCount++;
                }

            }

            if (curRatsCount <= 0)
            {
                return false;
            }

            SendStandardLetter(def.letterLabel, def.letterText, def.letterDef, parms, plant, $"{curRatsCount}".Colorize(UIUtility.YellowColor));
            return true;
        }

        private bool TryFindRandomBlightablePlant(Map map, out Plant plant)
        {
            Thing result;
            bool result2 = (from x in map.listerThings.ThingsInGroup(ThingRequestGroup.Plant)
                            where ((Plant)x).BlightableNow
                            select x).TryRandomElement(out result);
            plant = (Plant)result;
            return result2;
        }
    }
}
