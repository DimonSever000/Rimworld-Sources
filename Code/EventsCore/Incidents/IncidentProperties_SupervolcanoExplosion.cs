using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;
using Verse;

namespace EventsCore.Incidents
{
    public class IncidentProperties_SupervolcanoExplosion : IncidentProperties
    {
        public IntRange powerRange;

        public IntRange explodeRadius;

        public SimpleCurve ashByPowerCurve;
    }
}
