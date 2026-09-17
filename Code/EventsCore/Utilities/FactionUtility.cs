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

namespace EventsCore.Utilities
{
    [StaticConstructorOnStartup]
    public static class FactionUtility
    {
        public static bool AllyTo(this Faction fac, Faction other)
        {
            if (fac == null || other == null || other == fac)
            {
                return false;
            }

            return fac.RelationWith(other).kind == FactionRelationKind.Ally;
        }
    }
}
