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
    public class EducationSet
    {
        public List<EducationChance> educationChances = new List<EducationChance>();
        public bool TryGenerateEducation(out EducationDef education)
        {
            education = null;

            if (educationChances.NullOrEmpty())
            {
                return false;
            }

            if (!educationChances.TryRandomElementByWeight(x => x.chance, out EducationChance educationChance))
            {
                return false;
            }

            education = educationChance.education;

            return true;
        }

        public bool TryGetMaxEducation(out EducationDef education)
        {
            education = null;

            if (educationChances.NullOrEmpty())
            {
                return false;
            }

            if (!educationChances.TryMaxBy(x => x.education.maxResearchLevel, out EducationChance educationChance))
            {
                return false;
            }

            education = educationChance.education;

            return true;
        }
    }
}
