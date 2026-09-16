using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Diseases.Hediffs
{
    public class HediffCompProperties_Disease : HediffCompProperties
    {

        public IntRange intervalTicks;

        public FloatRange severityRange = new FloatRange(float.MinValue, float.MaxValue);

        public List<HediffDef> hediffDefs = new List<HediffDef>();

        public List<BodyPartDef> partsToAffect = new List<BodyPartDef>();

        public int countToAffect = 1;

        public bool letter = true;

        public float chance = 1f;
    }
}
