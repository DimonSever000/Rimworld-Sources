using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ScienceRework
{
    public class CompAbilityEffect_EducationValidation : CompAbilityEffect
    {
        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            Pawn pawn = target.Pawn;

            if (pawn != null)
            {
                if (!pawn.TryGetEducation(out EducationDef targetEducation))
                {
                    return false;
                }

                if (!parent.pawn.TryGetEducation(out EducationDef casterEducation))
                {
                    return false;
                }

                if (targetEducation.maxResearchLevel > casterEducation.maxResearchLevel)
                {
                    if (throwMessages)
                    {
                        Messages.Message("ScienceRework.CompAbilityEffect_EducationValidation.EducationReason".Translate(parent.pawn.LabelShort, pawn.LabelShort), MessageTypeDefOf.RejectInput, false);
                    }
                   
                    return false;
                }
            }

            return true;
        }
    }
}
