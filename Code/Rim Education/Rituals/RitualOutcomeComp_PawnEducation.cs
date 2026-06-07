using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ScienceRework.Rituals
{
    public class RitualOutcomeComp_PawnEducation : RitualOutcomeComp_QualitySingleOffset
    {
        [NoTranslate]
        public string roleId;

        public override float QualityOffset(LordJob_Ritual ritual, RitualOutcomeComp_Data data)
        {
            return Count(ritual, data);
        }

        protected float EducationValue(Pawn pawn)
        {
            if (curve == null)
            {
                return 0f;
            }

            if (!pawn.TryGetEducation(out EducationDef education))
            {
                return 0f;
            }

            return curve.Evaluate((int)education.maxResearchLevel);
        }

        public override float Count(LordJob_Ritual ritual, RitualOutcomeComp_Data data)
        {
            Pawn pawn = ritual.PawnWithRole(roleId);

            if (pawn == null)
            {
                return 0f;
            }

            return EducationValue(pawn);
        }

        public override string GetDesc(LordJob_Ritual ritual = null, RitualOutcomeComp_Data data = null)
        {
            if (ritual == null)
            {
                return labelAbstract;
            }

            Pawn pawn = ritual?.PawnWithRole(roleId);
            if (pawn == null)
            {
                return null;
            }

            float num = EducationValue(pawn);

            string text = ((num < 0f) ? "" : "+");
            return LabelForDesc.Formatted(pawn.Named("PAWN")) + ": " + "OutcomeBonusDesc_QualitySingleOffset".Translate(text + num.ToStringPercent()) + ".";
        }

        public override QualityFactor GetQualityFactor(Precept_Ritual ritual, TargetInfo ritualTarget, RitualObligation obligation, RitualRoleAssignments assignments, RitualOutcomeComp_Data data)
        {
            Pawn pawn = assignments.FirstAssignedPawn(roleId);
            if (pawn == null)
            {
                return null;
            }

            float num = EducationValue(pawn);

            if (!pawn.TryGetEducation(out EducationDef education))
            {
                return null;
            }

            return new QualityFactor
            {
                label = label.Formatted(pawn.Named("PAWN")),
                count = education.LabelCap,
                qualityChange = ((Math.Abs(num) > float.Epsilon) ? "OutcomeBonusDesc_QualitySingleOffset".Translate(num.ToStringWithSign("0.#%")).Resolve() : " - "),
                positive = (num >= 0f),
                quality = num,
                priority = 0f
            };
        }

        public override bool Applies(LordJob_Ritual ritual)
        {
            return true;
        }
    }
}
