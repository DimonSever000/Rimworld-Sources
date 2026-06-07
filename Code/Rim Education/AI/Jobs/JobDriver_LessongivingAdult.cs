using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace ScienceRework.AI.Jobs
{
    public class JobDriver_LessongivingAdult : JobDriver
    {
        public bool isReadyToTeach;
        private Thing Desk => base.TargetThingA;
        private Pawn Student => (Pawn)base.TargetThingB;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (!pawn.ReserveSittableOrSpot(SchoolUtility.DeskSpotTeacher(Desk), job, errorOnFailed))
            {
                return false;
            }

            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnDespawnedNullOrForbidden(TargetIndex.B);
            this.FailOn(delegate
            {
                if (PawnUtility.WillSoonHaveBasicNeed(pawn, -0.1f))
                {
                    return true;
                }

                Job job = Student?.jobs?.curJob;
                return (job == null || job.def != JobDefOfLocal.LessontakingAdult || job.GetTarget(TargetIndex.B).Pawn != pawn || job.GetTarget(TargetIndex.A).Thing != Desk) ? true : false;
            });

            yield return Toils_Goto.GotoCell(SchoolUtility.DeskSpotTeacher(Desk), PathEndMode.OnCell);

            Toil toil = ToilMaker.MakeToil("MakeNewToils");
            toil.tickIntervalAction = delegate (int delta)
            {
                pawn.GainComfortFromCellIfPossible(delta);
                isReadyToTeach = true;
                pawn.rotationTracker.FaceTarget(Desk);

                if (isReadyToTeach && Student?.jobs?.curDriver is JobDriver_LessontakingAdult jobDriver_LessontakingAdult && jobDriver_LessontakingAdult.isReadyToLearn)
                {
                    if (pawn.IsHashIntervalTick(900, delta))
                    {
                        pawn.interactions.TryInteractWith(Student, InteractionDefOfLocal.LessonIntellectual);
                        pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(ThoughtDefOf.GaveLesson, Student);
                        Student.needs?.mood?.thoughts?.memories?.TryGainMemory(ThoughtDefOf.WasTaught, pawn);
                    }

                    float amount = Utility.BasicLearnXpPerTick * delta;

                    if (Utility.TryLearnForEducation(Student, pawn, amount))
                    {
                        pawn.skills.Learn(SkillDefOf.Social, 0.1f * (float)delta);
                    }
                }
            };
            toil.handlingFacing = true;
            toil.defaultCompleteMode = ToilCompleteMode.Never;
            toil.socialMode = RandomSocialMode.Off;
            toil.activeSkill = () => SkillDefOf.Social;

            yield return toil;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref isReadyToTeach, "isReadyToTeach", defaultValue: false);
        }
    }
}
