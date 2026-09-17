using EventsCore.Incidents;
using EventsCore.World;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Analytics;
using UnityEngine.SocialPlatforms;
using Verse;

namespace EventsCore.World
{
    public class SitePartWorker_WorldMeteorite : SitePartWorker_PreciousLump
    {
        public override void PostDestroy(SitePart sitePart)
        {
            base.PostDestroy(sitePart);

            List<PlanetTile> tiles = new List<PlanetTile>();

            Find.WorldGrid.Surface.Filler.FloodFill(sitePart.site.Tile, (PlanetTile tile) => true, delegate (PlanetTile tile, int dist)
            {
                if (dist > IncidentWorker_WorldMeteorite.MeteoriteSize - 1)
                {
                    return true;
                }

                tiles.Add(tile);

                return false;
            });


            FeatureWorker_Special worker = FeatureDefOfLocal.MeteoriteCrater.Worker as FeatureWorker_Special;
            worker.AddFeatureDirect(Find.WorldGrid.Surface, tiles, tiles);
            Find.WorldFeatures.textsCreated = false;
        }
    }
}
