using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using Verse;

namespace EventsCore
{
    public class TechLevelRoad
    {
        public TechLevel techLevel;
        public RoadDef roadDef;
        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            techLevel = ParseHelper.FromString<TechLevel>(xmlRoot.Name);
            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "roadDef", xmlRoot.FirstChild.Value);
        }
    }
}
