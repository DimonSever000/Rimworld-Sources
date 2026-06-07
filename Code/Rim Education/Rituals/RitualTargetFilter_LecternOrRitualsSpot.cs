using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.Noise;

namespace ScienceRework.Rituals
{
    public class RitualTargetFilter_LecternOrRitualsSpot : RitualTargetFilter
    {
        public RitualTargetFilter_LecternOrRitualsSpot()
        {
        }

        public RitualTargetFilter_LecternOrRitualsSpot(RitualTargetFilterDef def) : base(def)
        {
        }

        public override bool CanStart(TargetInfo initiator, TargetInfo selectedTarget, out string rejectionReason)
        {
            TargetInfo targetInfo = BestTarget(initiator, selectedTarget);
            rejectionReason = "";

            if (!targetInfo.IsValid)
            {
                rejectionReason = "ScienceRework.RitualTargetFilter_LecternOrRitualsSpot.AbilityDisabledNoLecternOrRitualsSpot".Translate();
                return false;
            }

            return true;
        }

        public override TargetInfo BestTarget(TargetInfo initiator, TargetInfo selectedTarget)
        {
            if (!(initiator.Thing is Pawn pawn))
            {
                return null;
            }

            Thing thing = null;
            float num = 99999f;

            if (pawn.Map != null)
            {
                foreach (Building item in pawn.Map.listerBuildings.AllBuildingsColonistOfDef(ThingDefOf.Lectern))
                {
                    if (pawn.CanReach(item, PathEndMode.InteractionCell, pawn.NormalMaxDanger()))
                    {
                        int lengthHorizontalSquared = (pawn.Position - item.Position).LengthHorizontalSquared;
                        if ((float)lengthHorizontalSquared < num)
                        {
                            thing = item;
                            num = lengthHorizontalSquared;
                        }
                    }
                }

                if (thing == null)
                {
                    foreach (Thing item in pawn.Map.listerThings.ThingsOfDef(ThingDefOf.RitualSpot))
                    {
                        if (pawn.CanReach(item, PathEndMode.Touch, pawn.NormalMaxDanger()))
                        {
                            int lengthHorizontalSquared2 = (pawn.Position - item.Position).LengthHorizontalSquared;
                            if ((float)lengthHorizontalSquared2 < num)
                            {
                                thing = item;
                                num = lengthHorizontalSquared2;
                            }
                        }
                    }
                }
            }

            return thing;
        }

        public override IEnumerable<string> GetTargetInfos(TargetInfo initiator)
        {
            yield return ThingDefOf.RitualSpot.label;
            yield return ThingDefOf.Lectern.label;
        }
    }
}
