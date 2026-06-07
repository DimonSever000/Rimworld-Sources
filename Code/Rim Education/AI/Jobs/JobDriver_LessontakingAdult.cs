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
    public class JobDriver_LessontakingAdult : JobDriver
    {
        public bool isReadyToLearn;

        public int waitingForTeacherTicks;

        private Thing Desk => base.TargetThingA;
        private Pawn Teacher => (Pawn)base.TargetThingB;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.ReserveSittableOrSpot(SchoolUtility.DeskSpotStudent(Desk), job, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.AddEndCondition(delegate
            {
                if (!PawnUtility.WillSoonHaveBasicNeed(pawn, -0.1f))
                {
                    return JobCondition.Ongoing;
                }

                return JobCondition.Incompletable;
            });
            this.FailOn(delegate
            {
                if (Teacher == null)
                {
                    if (++waitingForTeacherTicks > 5000)
                    {
                        return true;
                    }
                    return false;
                }

                waitingForTeacherTicks = 0;
                Job job = Teacher.jobs?.curJob;

                return (job == null || job.def != JobDefOfLocal.LessongivingAdult || job.GetTarget(TargetIndex.B).Pawn != pawn || job.GetTarget(TargetIndex.A).Thing != Desk) ? true : false;
            });

            yield return Toils_Goto.GotoCell(SchoolUtility.DeskSpotStudent(Desk), PathEndMode.OnCell);

            Toil toil = ToilMaker.MakeToil("MakeNewToils");
            toil.tickIntervalAction = delegate (int delta)
            {
                pawn.GainComfortFromCellIfPossible(delta);
                isReadyToLearn = true;
                pawn.rotationTracker.FaceTarget(Desk);
            };
            toil.handlingFacing = true;
            toil.defaultCompleteMode = ToilCompleteMode.Never;
            toil.socialMode = RandomSocialMode.Off;

            yield return toil;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref isReadyToLearn, "isReadyToLearn", defaultValue: false);
            Scribe_Values.Look(ref waitingForTeacherTicks, "waitForTeacherTicks", 0);
        }
    }
}
