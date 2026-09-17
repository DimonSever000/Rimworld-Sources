using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.AI;
using EventsCore.GameConditions;

namespace EventsCore.Incidents
{
    public class IncidentWorker_ClimateChange : IncidentWorkerWithProps
    {
        public IncidentProperties_ClimateChange Props => props as IncidentProperties_ClimateChange;

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }

            GameConditionManager gameConditionManager = Find.World.GameConditionManager;
            if (gameConditionManager == null)
            {
                Log.ErrorOnce($"Couldn't find condition manager for incident target {parms.target}", 70849667);
                return false;
            }

            GameConditionDef gameConditionDef = def.gameCondition;
            if (gameConditionDef == null)
            {
                return false;
            }

            if (gameConditionManager.ConditionIsActive(def.gameCondition))
            {
                return false;
            }

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            StartClimateChange(def, null);

            SendStandardLetter(def.letterLabel, def.letterText, def.letterDef, parms, LookTargets.Invalid);

            return true;
        }

        public virtual void StartClimateChange(IncidentDef incidentDef, float? forcedTargetTemperatureChange = null, float? forcedTargetElevationChange = null)
        {
            GameConditionManager gameConditionManager = Find.World.GameConditionManager;
            GameConditionDef gameConditionDef = incidentDef.gameCondition;

            GameCondition_ClimateChange condition = Find.World.GameConditionManager.GetActiveCondition<GameCondition_ClimateChange>();
            if (condition == null)
            {
                condition = GameConditionMaker.MakeCondition(gameConditionDef) as GameCondition_ClimateChange;
                condition.forceDisplayAsDuration = true;
                gameConditionManager.RegisterCondition(condition);
            }

            condition.Initialize(incidentDef, forcedTargetTemperatureChange, forcedTargetElevationChange);
        }
    }
}
