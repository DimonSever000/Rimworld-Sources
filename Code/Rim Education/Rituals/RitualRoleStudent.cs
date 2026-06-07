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
    public class RitualRoleStudent : RitualRoleEducation
    {
        public override bool AppliesToPawn(Pawn p, out string reason, TargetInfo selectedTarget, LordJob_Ritual ritual = null, RitualRoleAssignments assignments = null, Precept_Ritual precept = null, bool skipReason = false)
        {
            if (!base.AppliesToPawn(p, out reason, selectedTarget, ritual, assignments, precept, skipReason))
            {
                return false;
            }

            if (!p.TryGetEducation(out EducationDef education) || education.next == null)
            {
                reason = "ScienceRework.RitualRoleStudent.MaxEducationReason".Translate();
                return false;
            }

            if (assignments != null)
            {
                RitualRole role = assignments.GetRole("teacher");
                foreach(Pawn teacher in assignments.AssignedPawns(role))
                {
                    if (!teacher.TryGetEducation(out EducationDef educationTeacher) || educationTeacher.maxResearchLevel <= education.maxResearchLevel)
                    {
                        reason = "ScienceRework.RitualRoleStudent.TeacherLowEducationReason".Translate();
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
