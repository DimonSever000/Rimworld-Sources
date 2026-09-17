using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Verse;

namespace EventsCore.Settings
{
    public class WorldSettings : Settings
    {
        public float volcanoesEruptionFactor = 1f;
        public float climateChangeRate = 1f;
        public float aiWorldBuildngSize = 1f;
        public float meteoriteOrePercent = 0.12f;
        public bool riverCoolingTexturing = true;
        public bool riverWarmingTexturing = false;
        public WorldSettings() : base()
        {

        }

        public override void ResetSettings()
        {
            volcanoesEruptionFactor = 1f;
            climateChangeRate = 1f;
            aiWorldBuildngSize = 1f;
            meteoriteOrePercent = 0.12f;
            riverCoolingTexturing = true;
            riverWarmingTexturing = true;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref volcanoesEruptionFactor, "volcanoesEruptionFactor", 1f);
            Scribe_Values.Look(ref climateChangeRate, "climateChangeRate", 1f);
            Scribe_Values.Look(ref aiWorldBuildngSize, "aiWorldBuildngSize", 1f);
            Scribe_Values.Look(ref meteoriteOrePercent, "meteoriteOrePercent", 0.12f);
            Scribe_Values.Look(ref riverCoolingTexturing, "riverCoolingTexturing", true);
            Scribe_Values.Look(ref riverWarmingTexturing, "riverWarmingTexturing", true);
        }
    }
}
