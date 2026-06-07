using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ScienceRework
{
    public class RoyalTitlePermitWorker_CallTeacher : RoyalTitlePermitWorker_CallLaborers
    {
        private static FieldInfo calledFactionInfo = AccessTools.Field(typeof(RoyalTitlePermitWorker_CallLaborers), "calledFaction");

        public override void OrderForceTarget(LocalTargetInfo target)
        {
            CallTeacher(target.Cell);
        }

        private void CallTeacher(IntVec3 landingCell)
        {
            Faction faction = (Faction)calledFactionInfo.GetValue(this);

            QuestScriptDef script = QuestScriptDefOfLocal.Permit_CallTeacher;

            Slate slate = new Slate();
            slate.Set("map", map);
            slate.Set("laborersCount", def.royalAid.pawnCount);
            slate.Set("permitFaction", faction);
            slate.Set("laborersPawnKind", def.royalAid.pawnKindDef);
            slate.Set("laborersDurationDays", def.royalAid.aidDurationDays);
            slate.Set("landingCell", landingCell);

            QuestUtility.GenerateQuestAndMakeAvailable(script, slate);
            caller.royalty.GetPermit(def, faction).Notify_Used();

            if (!free)
            {
                caller.royalty.TryRemoveFavor(faction, def.royalAid.favorCost);
            }
        }
    }
}
