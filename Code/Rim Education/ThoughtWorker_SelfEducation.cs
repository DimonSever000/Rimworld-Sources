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
    public class ThoughtWorker_SelfEducation : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            if (!p.TryGetEducation(out EducationDef education))
            {
                return ThoughtState.Inactive;
            }

            if (education == EducationDefOfLocal.Uneducated)
            {
                return ThoughtState.ActiveAtStage(0);
            }

            if (education == EducationDefOfLocal.Primary)
            {
                return ThoughtState.ActiveAtStage(1);
            }

            if (education == EducationDefOfLocal.Basic)
            {
                return ThoughtState.ActiveAtStage(2);
            }

            if (education == EducationDefOfLocal.Higher)
            {
                return ThoughtState.ActiveAtStage(3);
            }

            if (education == EducationDefOfLocal.Academic)
            {
                return ThoughtState.ActiveAtStage(4);
            }

            return ThoughtState.ActiveAtStage(5);
        }
    }
}
