using EventsCore.Utilities;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace EventsCore.Quests
{
    public class QuestNode_Root_WandererJoin_OldMan : QuestNode_Root_WandererJoin_WalkIn
    {
        private static readonly FieldInfo signalAccept = AccessTools.Field(typeof(QuestNode_Root_WandererJoin_WalkIn), "signalAccept");
        private static readonly FieldInfo signalReject = AccessTools.Field(typeof(QuestNode_Root_WandererJoin_WalkIn), "signalReject");
        public override Pawn GeneratePawn()
        {
            PawnGenerationRequest pawnGenerationRequest = new PawnGenerationRequest(IncidentDefOfLocal.OldMan.pawnKind, null, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: true, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 20f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: true, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Adult, null, null, null, forceRecruitable: true);
            Pawn pawn = PawnGenerator.GeneratePawn(pawnGenerationRequest);
            
            if (!pawn.IsWorldPawn())
            {
                Find.WorldPawns.PassToWorld(pawn);
            }

            return pawn;
        }
        public override void SendLetter(Quest quest, Pawn pawn)
        {
            TaggedString title = IncidentDefOfLocal.OldMan.letterLabel.Formatted(pawn.LabelShort).Resolve();
            TaggedString letterText = IncidentDefOfLocal.OldMan.letterText.Formatted(pawn.LabelShort.Colorize(UIUtility.YellowColor)).Resolve();

            AppendCharityInfoToLetter("JoinerCharityInfo".Translate(pawn), ref letterText);
            PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref letterText, ref title, pawn);

            ChoiceLetter_AcceptJoiner choiceLetter_AcceptJoiner = (ChoiceLetter_AcceptJoiner)LetterMaker.MakeLetter(title, letterText, LetterDefOf.AcceptJoiner);
            choiceLetter_AcceptJoiner.signalAccept = (string)signalAccept.GetValue(this);
            choiceLetter_AcceptJoiner.signalReject = (string)signalReject.GetValue(this);
            choiceLetter_AcceptJoiner.quest = quest;
            choiceLetter_AcceptJoiner.StartTimeout(60000);

            Find.LetterStack.ReceiveLetter(choiceLetter_AcceptJoiner);
        }
    }
}
