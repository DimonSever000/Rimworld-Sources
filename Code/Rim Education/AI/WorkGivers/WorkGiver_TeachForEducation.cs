using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace ScienceRework.AI.WorkGivers
{
    public class WorkGiver_TeachForEducation : WorkGiver_Scanner
    {
        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn teacher)
        {
            return teacher.Map.mapPawns.FreeColonists;
        }

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            if (ModsConfig.BiotechActive)
            {
                return !StudentUtility.CanTeachNow(pawn);
            }

            return true;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!(t is Pawn student))
            {
                return false;
            }

            if (StudentUtility.NeedsTeacher(student))
            {
                if (StudentUtility.CanTeachNow(pawn, student))
                {
                    Thing thing = student.CurJob.GetTarget(TargetIndex.A).Thing;

                    if (thing != null && !thing.Destroyed && thing.Spawned && pawn.CanReserveSittableOrSpot(SchoolUtility.DeskSpotTeacher(thing)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!(t is Pawn student))
            {
                return null;
            }

            student.CurJob.SetTarget(TargetIndex.B, pawn);
            return JobMaker.MakeJob(JobDefOfLocal.LessongivingAdult, student.CurJob.GetTarget(TargetIndex.A), student);
        }
    }
}
