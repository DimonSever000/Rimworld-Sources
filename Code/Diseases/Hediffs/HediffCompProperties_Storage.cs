using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Diseases.Hediffs
{
    public class HediffCompProperties_Storage : HediffCompProperties
    {
        public List<HediffCompProperties_Disease> comps;
        public HediffCompProperties_Storage()
        {
            compClass = typeof(HediffComp_Storage);
        }
    }
}
