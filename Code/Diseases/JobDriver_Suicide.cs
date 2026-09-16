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
    public class JobDriver_Suicide : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A).FailOnDowned(TargetIndex.A);

            yield return Toils_General.Wait(500).WithProgressBarToilDelay(TargetIndex.A).FailOnDowned(TargetIndex.A).FailOnDestroyedNullOrForbidden(TargetIndex.A);
            yield return new Toil()
            {
                initAction = delegate
                {
                    pawn.TakeDamage(new DamageInfo(DamageDefOf.Scratch, 10, 99999, instigator: pawn));
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            }.FailOnDowned(TargetIndex.A);
        }
    }
}
