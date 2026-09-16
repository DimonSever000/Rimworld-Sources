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
    public class MentalBreakWorker_Suicide : MentalBreakWorker
    {
        public override float CommonalityFor(Pawn pawn, bool moodCaused = false)
        {
            float commonality = base.CommonalityFor(pawn, moodCaused);
            Trait naturalMoodTrait = pawn.story?.traits?.GetTrait(TraitDefOfLocal.NaturalMood);
            if (naturalMoodTrait != null && naturalMoodTrait.Degree < 0)
            {
                commonality *= 2.5f;
            }
            return commonality;
        }
    }
}
