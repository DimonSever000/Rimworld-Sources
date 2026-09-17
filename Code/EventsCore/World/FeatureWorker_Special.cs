using RimWorld.Planet;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EventsCore.World
{
    public class FeatureWorker_Special : FeatureWorker
    {
        public void AddFeatureDirect(PlanetLayer layer, List<PlanetTile> members, List<PlanetTile> tilesForTextDrawPosCalculation)
        {
            AddFeature(layer, members, tilesForTextDrawPosCalculation);
        }

        public override void GenerateWhereAppropriate(PlanetLayer layer)
        {
            
        }
    }
}
