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
    public class RitualRoleTeacher : RitualRoleEducation
    {
        public override bool AppliesToPawn(Pawn p, out string reason, TargetInfo selectedTarget, LordJob_Ritual ritual = null, RitualRoleAssignments assignments = null, Precept_Ritual precept = null, bool skipReason = false)
        {
            if (!base.AppliesToPawn(p, out reason, selectedTarget, ritual, assignments, precept, skipReason))
            {
                return false;
            }

            if (!p.TryGetEducation(out EducationDef education) || education == EducationDefOfLocal.Uneducated)
            {
                reason = "ScienceRework.RitualRoleTeacher.UneducatedTeacherReason".Translate();
                return false;
            }

            return true;
        }

        protected override int PawnDesirability(Pawn pawn)
        {
            int num = base.PawnDesirability(pawn);

            if (pawn.TryGetEducation(out EducationDef education) && education != EducationDefOfLocal.Uneducated)
            {
                num += 100 * (int)education.maxResearchLevel;
            }

            return num;
        }
    }
}
