using EventsCore.Incidents;
using EventsCore.Utilities;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace EventsCore.GameConditions
{
    public class GameCondition_DeathOfSun : GameCondition, ISunLuminosityInfluencer
    {
        private float curSunLuminosityFactor = 1f;
        public override string Description => base.Description.Formatted(curSunLuminosityFactor.ToStringPercent().Colorize(UIUtility.YellowColor)).Resolve();
        public override void Init()
        {
            base.Init();

            curSunLuminosityFactor = 1f;
            this.Permanent = true;
            MiscUtility.Notify_SunLuminosityChanged();

            IncidentParms parms = StorytellerUtility.DefaultParmsNow(IncidentDefOfLocal.GlobalCooling.category, Find.AnyPlayerHomeMap);
            IncidentDefOfLocal.GlobalCooling.Worker.TryExecute(parms);
            //IncidentWorker_ClimateChange.StartClimateChange(IncidentDefOfLocal.GlobalCooling, -200f);
        }

        public override void GameConditionTick()
        {
            base.GameConditionTick();

            if (Find.TickManager.TicksGame % 60000 == 0)
            {
                curSunLuminosityFactor = Mathf.Max(0f, curSunLuminosityFactor - Rand.Range(0.05f, 0.1f));
                MiscUtility.Notify_SunLuminosityChanged();
            }
        }

        public float SunLuminosityFactor()
        {
            return curSunLuminosityFactor;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref curSunLuminosityFactor, "curSunLuminosityFactor");
        }
    }
}
