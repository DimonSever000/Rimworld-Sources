using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static UnityEngine.Networking.UnityWebRequest;

namespace ScienceRework
{
    public class ScenPart_ForcedEducation : ScenPart_PawnModifier
    {
        protected EducationDef forcedEducation;

        public override string Summary(Scenario scen)
        {
            return $"{Label}: {forcedEducation.LabelCap}";
        }

        public override void Randomize()
        {
            base.Randomize();
            forcedEducation = DefDatabase<EducationDef>.GetRandom();
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            Rect scenPartRect = listing.GetScenPartRect(this, ScenPart.RowHeight);
            DoEducationEditInterface(scenPartRect);
        }

        private void DoEducationEditInterface(Rect rect)
        {
            if (Widgets.ButtonText(rect, forcedEducation.LabelCap))
            {
                FloatMenuUtility.MakeMenu(DefDatabase<EducationDef>.AllDefs, x => x.LabelCap, x => delegate
                {
                    forcedEducation = x;
                });
            }
        }

        public override bool CanCoexistWith(ScenPart other)
        {
            if (other is ScenPart_ForcedEducation scenPart)
            {
                return false;
            }

            return true;
        }

        public override bool TryMerge(ScenPart other)
        {
            if (other is ScenPart_ForcedEducation scenPart)
            {
                return scenPart.forcedEducation == forcedEducation;
            }

            return false;
        }

        public override void Notify_NewPawnGenerating(Pawn pawn, PawnGenerationContext context)
        {
            if (context == PawnGenerationContext.PlayerStarter && Rand.Chance(chance) && pawn.RaceProps.Humanlike)
            {
                pawn.TrySetEducation(forcedEducation);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Defs.Look(ref forcedEducation, "forcedEducation");
        }
    }
}
