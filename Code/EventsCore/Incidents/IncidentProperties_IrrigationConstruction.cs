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
    public class IncidentProperties_IrrigationConstruction : IncidentProperties_WorldConstruction
    {
        /// <summary>
        /// Отражает размеры реки, которую может продлить фракция для тех. уровня
        /// </summary>
        public SimpleCurve riverLengthByTechLevelCurve;
    }
}
