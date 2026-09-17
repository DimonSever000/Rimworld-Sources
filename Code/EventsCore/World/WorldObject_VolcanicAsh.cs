using LudeonTK;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace EventsCore.World
{
    public class WorldObject_VolcanicAsh : WorldObject
    {
        public int lifeSpan = 900000;

        public int destinationTile = -1;

        public int initialTile = -1;

        private float traveledPct;

        public int period = 30000;

        private Vector3 Start => Find.WorldGrid.GetTileCenter(initialTile);

        private Vector3 End => Find.WorldGrid.GetTileCenter(destinationTile);

        public override Vector3 DrawPos => Vector3.Slerp(Start, End, traveledPct);

        public override void PostAdd()
        {
            base.PostAdd();

            initialTile = base.Tile;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref lifeSpan, "lifeSpan", 900000);
            Scribe_Values.Look(ref initialTile, "initialTile", 0);
            Scribe_Values.Look(ref traveledPct, "traveledPct", 0f);
            Scribe_Values.Look(ref destinationTile, "destinationTile", 0);
        }

        protected override void Tick()
        {
            base.Tick();
            if (Tile == -1)
            {
                Find.WorldObjects.Remove(this);
            }

            traveledPct += 1f / (float)period;
            if (traveledPct >= 1f)
            {
                traveledPct = 1f;
                Arrived();
            }

            lifeSpan--;
            if (lifeSpan == 0)
            {
                Find.WorldObjects.Remove(this);
            }
        }

        private void Arrived()
        {
            Tile = destinationTile;
            traveledPct = 0f;

            Map map = Current.Game.FindMap(Tile);

            if (map != null && !map.GameConditionManager.ConditionIsActive(GameConditionDefOf.VolcanicWinter))
            {
                IncidentDefOfLocal.VolcanicWinter.Worker.TryExecute(StorytellerUtility.DefaultParmsNow(IncidentDefOfLocal.VolcanicWinter.category, map));
            }

            Destroy();
        }
    }
}
