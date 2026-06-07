using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ScienceRework
{
    public class ThoughtWorker_EducationDifference : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            if (!p.RaceProps.Humanlike)
            {
                return false;
            }

            if (!other.RaceProps.Humanlike)
            {
                return false;
            }

            if (ThoughtUtility.ThoughtNullified(p, def))
            {
                return false;
            }

            if (!RelationsUtility.PawnsKnowEachOther(p, other))
            {
                return false;
            }

            if (!p.TryGetEducation(out EducationDef education1))
            {
                return false;
            }

            if (!other.TryGetEducation(out EducationDef education2))
            {
                return false;
            }

            if (education1.maxResearchLevel != education2.maxResearchLevel)
            {
                return ThoughtState.ActiveAtStage(1);
            }

            return ThoughtState.ActiveAtStage(0);
        }
    }
}
