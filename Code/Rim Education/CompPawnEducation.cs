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
    public class CompPawnEducation : ThingComp
    {
        public Pawn pawn => parent as Pawn;

        private EducationDef education;
        private EducationDef Education
        {
            get
            {
                if (education == null)
                {
                    education = EducationDefOfLocal.Uneducated;
                }

                return education;
            }
        }

        private float educationProgress;
        public float EducationProgress => educationProgress;

        public bool Learn(float amount, Pawn teacher)
        {
            if (!ShouldEverHaveEducation())
            {
                return false;
            }

            if (Education.next == null)
            {
                educationProgress = 0f;
                return false;
            }

            if (!Education.developmentalStageFilter.Has(pawn.DevelopmentalStage))
            {
                educationProgress = 0f;
                return false;
            }

            if (teacher != null)
            {
                if (!teacher.TryGetEducation(out EducationDef teacherEducation))
                {
                    return false;
                }

                if (Education.next.maxResearchLevel > teacherEducation.maxResearchLevel)
                {
                    return false;
                }
            }

            amount *= 1f / (1f + Education.next.difficulty);
            amount *= EducationRateFactor(teacher, pawn);
            amount *= Utility.Settings.educationSpeed;

            if (amount <= 0)
            {
                return false;
            }

            educationProgress += amount;
            educationProgress = Mathf.Clamp01(educationProgress);

            if (educationProgress >= 1f)
            {
                if (TrySetEducation(Education.next))
                {
                    Messages.Message($"ScienceRework.CompPawnEducation.EducationAttained".Translate(pawn.LabelShort, Education.prev.LabelCap, Education.LabelCap), MessageTypeDefOf.PositiveEvent);
                    educationProgress = 0f;
                }
            }

            return true;
        }

        public bool TryGetEducation(out EducationDef education)
        {
            if (!ShouldEverHaveEducation())
            {
                education = EducationDefOfLocal.Uneducated;
                return false;
            }

            education = Education;
            return true;
        }

        public bool TrySetEducation(EducationDef education)
        {
            if (!ShouldEverHaveEducation())
            {
                return false;
            }

            if (education == null)
            {
                return false;
            }

            this.education = education;
            pawn.skills?.DirtyAptitudes();

            return true;
        }

        public bool ShouldEverHaveEducation()
        {
            if (!pawn.RaceProps.Humanlike)
            {
                return false;
            }

            if (pawn.RaceProps.IsMechanoid)
            {
                return false;
            }

            if (pawn.RaceProps.IsAnomalyEntity)
            {
                return false;
            }

            if (pawn.IsMutant)
            {
                return false;
            }

            return true;
        }

        public static float EducationRateFactor(Pawn teacher, Pawn student)
        {
            float educationSpeed = student.GetStatValue(StatDefOfLocal.EducationSpeed);
            float educatingSpeed = teacher == null ? 1f : teacher.GetStatValue(StatDefOfLocal.EducatingSpeed);

            return educationSpeed * educatingSpeed;
        }


        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Defs.Look(ref education, "education");
            Scribe_Values.Look(ref educationProgress, "educationProgress");
        }
    }
}
