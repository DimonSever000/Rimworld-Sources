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
    public class RoleRequirement_MinEducationTechLevel : RoleRequirement
    {
        public TechLevel minEducationTechLevel;

        [NoTranslate]
        private string labelCached;

        private EducationDef minEducationCached;
        private EducationDef MinEducationCached
        {
            get
            {
                if (minEducationCached == null)
                {
                    if (!Utility.TryGetMinEducationForTechLevel(minEducationTechLevel, out minEducationCached))
                    {
                        minEducationCached = EducationDefOfLocal.Uneducated;
                    }
                }

                return minEducationCached;
            }
        }

        public override string GetLabel(Precept_Role role)
        {
            if (labelCached == null)
            {
                labelCached = "ScienceRework.RoleRequirement_MinEducationTechLevel.GetLabel".Translate(MinEducationCached.label);
            }

            return labelCached;
        }

        public override bool Met(Pawn p, Precept_Role role)
        {
            if (!p.TryGetEducation(out EducationDef education))
            {
                return false;
            }

            if (education.maxResearchLevel < MinEducationCached.maxResearchLevel)
            {
                return false;
            }

            return true;
        }
    }
}
