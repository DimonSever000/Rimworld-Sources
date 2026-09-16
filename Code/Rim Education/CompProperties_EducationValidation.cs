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
    public class CompProperties_EducationValidation : CompProperties_EffectWithDest
    {
        public CompProperties_EducationValidation()
        {
            compClass = typeof(CompAbilityEffect_EducationValidation);
        }
    }
}
