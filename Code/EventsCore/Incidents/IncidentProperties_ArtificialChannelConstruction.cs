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
    public class IncidentProperties_ArtificialChannelConstruction : IncidentProperties_WorldConstruction
    {
        /// <summary>
        /// Отражает размеры канала, который может быть построен для тех. уровня фракции
        /// </summary>
        public SimpleCurve channelLengthByTechLevelCurve;

        public IntRange defaultChannelLength;
    }
}
