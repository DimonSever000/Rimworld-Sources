using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace EventsCore.Incidents
{
    public class DefModExtension_IncidentProps : DefModExtension
    {
        public bool activeByDefault = true;
        public IncidentProperties props;
    }
}
