using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ScienceRework.Rituals
{
    public class RitualObligationTargetWorker_RitualSpotOrLectern : RitualObligationTargetWorker_ThingDef
    {
        public RitualObligationTargetWorker_RitualSpotOrLectern()
        {
        }

        public RitualObligationTargetWorker_RitualSpotOrLectern(RitualObligationTargetFilterDef def) : base(def)
        {
        }

        protected override RitualTargetUseReport CanUseTargetInternal(TargetInfo target, RitualObligation obligation)
        {
            if (!base.CanUseTargetInternal(target, obligation).canUse)
            {
                return false;
            }

            return true;
        }

        public override IEnumerable<string> GetTargetInfos(RitualObligation obligation)
        {
            yield return ThingDefOf.RitualSpot.label;
            yield return ThingDefOf.Lectern.label;
        }
    }
}
