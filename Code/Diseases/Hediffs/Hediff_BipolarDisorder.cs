using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Diseases.Hediffs
{
    public class Hediff_BipolarDisorder : HediffWithComps
    {
        public override void Tick()
        {
            base.Tick();

            if (Find.TickManager.TicksGame % 30000 == 0)
            {
                TryChangeStage();
            }
        }

        private void TryChangeStage()
        {
            if (Rand.Chance(0.33f))
            {
                if (Severity >= def.maxSeverity)
                {
                    Severity = def.initialSeverity;
                }
                else
                {
                    Severity = def.maxSeverity;
                }
            }
        }
    }
}
