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
    public class RitualOutcomeComp_ParticipantCountColorized : RitualOutcomeComp_ParticipantCount
    {
        public override QualityFactor GetQualityFactor(Precept_Ritual ritual, TargetInfo ritualTarget, RitualObligation obligation, RitualRoleAssignments assignments, RitualOutcomeComp_Data data)
        {
            int num = assignments.Participants.Count((Pawn p) => Counts(assignments, p));
            float quality = curve.Evaluate(num);

            return new QualityFactor
            {
                label = label.CapitalizeFirst(),
                count = num + " / " + Mathf.Max(base.MaxValue, num),
                qualityChange = ((Math.Abs(quality) > float.Epsilon) ? "OutcomeBonusDesc_QualitySingleOffset".Translate(quality.ToStringWithSign("0.#%")).Resolve() : " - "),
                quality = quality,
                positive = quality >= 0,
                priority = 4f
            };
        }
    }
}
