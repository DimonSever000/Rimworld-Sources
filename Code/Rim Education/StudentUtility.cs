using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace ScienceRework
{
    public static class StudentUtility
    {
        private static List<Pawn> teachers = new List<Pawn>();
        public static List<Pawn> SpawnedColonistTeachers(Map map)
        {
            teachers.Clear();

            foreach (Pawn item in map.mapPawns.FreeColonistsSpawned)
            {
                if (!item.WorkTypeIsDisabled(WorkTypeDefOf.Research))
                {
                    teachers.Add(item);
                }
            }

            return teachers;
        }

        public static Pawn FindTeacher(Pawn student)
        {
            foreach (Pawn teacher in SpawnedColonistTeachers(student.Map))
            {
                if (CanTeachNow(teacher, student) && teacher.CanReach(student, PathEndMode.Touch, Danger.Deadly))
                {
                    return teacher;
                }
            }

            return null;
        }

        public static bool CanTeachNow(Pawn teacher)
        {
            if (!teacher.DevelopmentalStage.Adult() || teacher.Downed || teacher.Drafted || !teacher.Awake() || teacher.IsBurning() || teacher.InMentalState || teacher.GetLord() != null ||
                teacher.IsPrisoner || teacher.IsSlave || teacher.IsSubhuman || teacher.Inhumanized() ||
                teacher.WorkTypeIsDisabled(WorkTypeDefOf.Research) || !teacher.workSettings.WorkIsActive(WorkTypeDefOf.Research) || !teacher.health.capacities.CapableOf(PawnCapacityDefOf.Talking) ||
                PawnUtility.WillSoonHaveBasicNeed(teacher, -0.1f))
            {
                return false;
            }

            if (!teacher.TryGetEducation(out EducationDef education) || education == EducationDefOfLocal.Uneducated)
            {
                return false;
            }

            return true;
        }

        public static bool CanLearnNow(Pawn student)
        {
            if (student.Downed || student.Drafted || !student.Awake() || student.IsBurning() || student.InMentalState || student.GetLord() != null ||
                student.IsPrisoner || student.IsSlave || student.IsSubhuman || student.Inhumanized() ||
                student.WorkTypeIsDisabled(WorkTypeDefOfLocal.LearningForEducation) || !student.workSettings.WorkIsActive(WorkTypeDefOfLocal.LearningForEducation) || !student.health.capacities.CapableOf(PawnCapacityDefOf.Talking) ||
                PawnUtility.WillSoonHaveBasicNeed(student, -0.1f))
            {
                return false;
            }

            if (!student.TryGetEducation(out EducationDef education) || education.next == null)
            {
                return false;
            }

            return true;
        }

        public static IEnumerable<Pawn> FindStudentsForTeacher(Pawn teacher)
        {
            foreach(Pawn student in SpawnedColonistTeachers(teacher.MapHeld))
            {
                if (CanLearnNow(student) && CanTeachNow(teacher, student))
                {
                    yield return student;
                }
            }
        }

        public static bool CanTeachNow(Pawn teacher, Pawn student)
        {
            if (teacher == student)
            {
                return false;
            }

            if (!CanTeachNow(teacher))
            {
                return false;
            }

            if (teacher.CurJobDef == JobDefOfLocal.LessongivingAdult || teacher.CurJobDef == JobDefOf.Lessongiving)
            {
                return false;
            }

            if (!teacher.TryGetEducation(out EducationDef educationTeacher) || !student.TryGetEducation(out EducationDef educationStudent))
            {
                return false;
            }

            if (educationStudent.maxResearchLevel >= educationTeacher.maxResearchLevel)
            {
                return false;
            }

            return true;
        }

        public static bool NeedsTeacher(Pawn student)
        {
            if (student.CurJobDef != JobDefOfLocal.LessontakingAdult)
            {
                return false;
            }

            Pawn pawn = student.CurJob.GetTarget(TargetIndex.B).Pawn;
            if (pawn != null && pawn.CurJobDef == JobDefOfLocal.LessongivingAdult && pawn.CurJob.GetTarget(TargetIndex.B) == student)
            {
                return false;
            }

            return true;
        }
    }
}
