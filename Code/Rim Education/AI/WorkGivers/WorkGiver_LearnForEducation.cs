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
    public class WorkGiver_LearnForEducation : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(ThingDefOf.SchoolDesk);

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            if (!ModsConfig.BiotechActive)
            {
                return true;
            }

            if (!StudentUtility.CanLearnNow(pawn))
            {
                return true;
            }

            return false;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t == null || !pawn.CanReserveSittableOrSpot(SchoolUtility.DeskSpotStudent(t)))
            {
                return false;
            }

            Pawn teacher = StudentUtility.FindTeacher(pawn);
            if (teacher == null || !teacher.CanReserveSittableOrSpot(SchoolUtility.DeskSpotTeacher(t)))
            {
                return false;
            }

            if (!StudentUtility.CanTeachNow(teacher, pawn))
            {
                return false;
            }

            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(JobDefOfLocal.LessontakingAdult, t);
        }
    }
}
