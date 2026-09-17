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

namespace EventsCore.Incidents
{
    public class IncidentWorker_MakeGameConditionWorld : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            GameConditionManager gameConditionManager = Find.World.GameConditionManager;

            if (gameConditionManager.ConditionIsActive(def.gameCondition))
            {
                return false;
            }

            List<GameCondition> activeConditions = gameConditionManager.ActiveConditions;
            for (int i = 0; i < activeConditions.Count; i++)
            {
                if (!def.gameCondition.CanCoexistWith(activeConditions[i].def))
                {
                    return false;
                }
            }

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            GameConditionManager gameConditionManager = Find.World.GameConditionManager;
            GameConditionDef gameConditionDef = def.gameCondition;

            int duration = Mathf.RoundToInt(def.durationDays.RandomInRange * 60000f);
            GameCondition condition = GameConditionMaker.MakeCondition(gameConditionDef, duration);
            gameConditionManager.RegisterCondition(condition);

            SendStandardLetter(def.letterLabel, def.letterText, def.letterDef, parms, LookTargets.Invalid);

            return true;
        }
    }
}
