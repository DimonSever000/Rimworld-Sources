using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Diseases.Hediffs
{
    public class HediffComp_DiseaseAfterTicks : HediffComp_Disease
    {
        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            DoEffect(ref severityAdjustment);
        }

        protected override void DoEffect(ref float severityAdjustment)
        {
            if (IsReady(ref severityAdjustment))
            {
                if (TryApply(ref severityAdjustment, out HediffDef hediffDef) && hediffDef != null && Props.letter)
                {
                    SendLetter(hediffDef);
                }
            }
        }
        protected override bool IsReady(ref float severityAdjustment)
        {
            if (base.IsReady(ref severityAdjustment))
            {
                if (!Props.severityRange.Includes(severityAdjustment))
                {
                    UpdateData();
                    return false;
                }
                if (Pawn.health.hediffSet.GetNotMissingParts().ToList().Find(part => Props.partsToAffect.Contains(part.def)) == null)
                {
                    UpdateData();
                    return false;
                }
                foreach (HediffDef hediffDef in Props.hediffDefs)
                {
                    if (parent.pawn.health.hediffSet.HasHediff(hediffDef))
                    {
                        UpdateData();
                        return false;
                    }
                }
                if (Tick < startTick + intervalTicks)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        protected virtual bool TryApply(ref float severityAdjustment, out HediffDef selectedHediffDef)
        {
            Props.hediffDefs.TryRandomElement(out HediffDef hediffDef);
            selectedHediffDef = hediffDef;
            Pawn pawn = parent.pawn;
            if (!Rand.Chance(Props.chance))
            {
                return false;
            }
            if (hediffDef == null || pawn == null)
            {
                return false;
            }
            if (Props.partsToAffect != null)
            {
                bool result = false;
                for (int i = 0; i < Props.countToAffect; i++)
                {
                    IEnumerable<BodyPartRecord> source = pawn.health.hediffSet.GetNotMissingParts();
                    if (Props.partsToAffect != null)
                    {
                        source = source.Where((BodyPartRecord p) => Props.partsToAffect.Contains(p.def));
                    }
                    source = source.Where((BodyPartRecord p) => !pawn.health.hediffSet.HasHediff(hediffDef, p) && !pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(p));
                    if (!source.Any())
                    {
                        break;
                    }
                    BodyPartRecord partRecord = source.RandomElementByWeight((BodyPartRecord x) => x.coverageAbs);
                    Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn, partRecord);
                    pawn.health.AddHediff(hediff);
                    result = true;
                }
                return result;
            }
            if (!pawn.health.hediffSet.HasHediff(hediffDef))
            {
                Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn);
                pawn.health.AddHediff(hediff);
                return true;
            }
            return false;
        }

        protected override void SendLetter(HediffDef hediffDef)
        {
            if (PawnUtility.ShouldSendNotificationAbout(parent.pawn))
            {
                Find.LetterStack.ReceiveLetter("LetterHealthComplicationsLabel".Translate(parent.pawn.LabelShort, hediffDef.LabelCap, parent.pawn.Named("PAWN")).CapitalizeFirst(), "LetterHealthComplications".Translate(parent.pawn.LabelShortCap, hediffDef.LabelCap, parent.LabelCap, parent.pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.NegativeEvent, parent.pawn);
            }
        }
    }
}
