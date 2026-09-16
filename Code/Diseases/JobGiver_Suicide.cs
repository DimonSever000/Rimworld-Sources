using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;

namespace Diseases
{
    public class JobGiver_Suicide : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            Job job = JobMaker.MakeJob(JobDefOfLocal.DimonSever000_Suicide, pawn);
            return job;
        }
    }
}
