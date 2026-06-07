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
    public abstract class RitualRoleEducation : RitualRoleColonist
    {
        public override bool AppliesToPawn(Pawn p, out string reason, TargetInfo selectedTarget, LordJob_Ritual ritual = null, RitualRoleAssignments assignments = null, Precept_Ritual precept = null, bool skipReason = false)
        {
            if (!base.AppliesToPawn(p,  out reason, selectedTarget, ritual, assignments, precept, skipReason))
            {
                return false;
            }

            if (!p.TryGetEducation(out EducationDef education))
            {
                return false;
            }

            return true;
        }
    }
}
