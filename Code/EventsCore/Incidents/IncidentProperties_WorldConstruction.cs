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
    public class IncidentProperties_WorldConstruction : IncidentProperties
    {
        public TechLevel minTechLevel = TechLevel.Undefined;
    }
}
