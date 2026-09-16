using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;

namespace Diseases
{
    public class RecipeWorker_Lobotomy : Recipe_Surgery
    {
        public override bool IsViolationOnPawn(Pawn pawn, BodyPartRecord part, Faction billDoerFaction)
        {
            if ((pawn.Faction == billDoerFaction || pawn.Faction == null) && !pawn.IsQuestLodger())
            {
                return false;
            }
            return true;
        }
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            return MedicalRecipesUtility.GetFixedPartsToApplyOn(recipe, pawn, delegate (BodyPartRecord record)
            {
                IEnumerable<Hediff> source = pawn.health.hediffSet.hediffs.Where((Hediff x) => x.Part == record);
                if (record.parent != null && !pawn.health.hediffSet.GetNotMissingParts().Contains(record.parent))
                {
                    return false;
                }
                return (!pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(record) || pawn.health.hediffSet.HasDirectlyAddedPartFor(record)) ? true : false;
            });
        }
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            bool flag = MedicalRecipesUtility.IsClean(pawn, part);
            bool flag2 = IsViolationOnPawn(pawn, part, Faction.OfPlayer);
            if (billDoer != null)
            {
                if (CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
                {
                    return;
                }
                TaleRecorder.RecordTale(TaleDefOf.DidSurgery, billDoer, pawn);
            }
            TryHealPawn(pawn, part);
            if (flag2)
            {
                ReportViolation(pawn, billDoer, pawn.HomeFaction, -70);
            }
        }

        public virtual void TryHealPawn(Pawn pawn, BodyPartRecord part)
        {
            Hediff hediff = pawn.health.hediffSet.hediffs?.Find(x => x.Part == part && x.def.chronic && x.def.isBad && !(x is Hediff_Injury) && !(x is Hediff_MissingPart));
            if (hediff != null)
            {
                Messages.Message("Diseases.RecipeWorker_Lobotomy.Successful".Translate(hediff.LabelCap), pawn, MessageTypeDefOf.PositiveEvent);
                pawn.health.RemoveHediff(hediff);
            }
        }
    }
}
