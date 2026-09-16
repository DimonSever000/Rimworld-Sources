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
    public class HediffGiver_RandomStressCurved : HediffGiver
    {
        public float OnBreakThresholdExtreme = -1f;
        public float OnBreakThresholdMajor = -1f;
        public float OnBreakThresholdMinor = -1f;

        public override float ChanceFactor(Pawn pawn)
        {
            if (pawn.IsMutant)
            {
                return 0f;
            }

            if (pawn.mindState?.mentalBreaker == null)
            {
                return 0f;
            }

            if (pawn.needs?.mood == null)
            {
                return 0f;
            }

            return base.ChanceFactor(pawn);
        }

        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            float value = -1f;

            if (pawn.mindState.mentalBreaker.CurMood <= pawn.mindState.mentalBreaker.BreakThresholdExtreme)
            {
                value = OnBreakThresholdExtreme;
            }
            else if (pawn.mindState.mentalBreaker.CurMood <= pawn.mindState.mentalBreaker.BreakThresholdMajor)
            {
                value = OnBreakThresholdMajor;
            }
            else if (pawn.mindState.mentalBreaker.CurMood <= pawn.mindState.mentalBreaker.BreakThresholdMinor)
            {
                value = OnBreakThresholdMinor;
            }

            if (value > 0f && Rand.MTBEventOccurs(value, 60000f, 60f) && TryApply(pawn))
            {
                SendLetter(pawn, cause);
            }
        }
    }
}
