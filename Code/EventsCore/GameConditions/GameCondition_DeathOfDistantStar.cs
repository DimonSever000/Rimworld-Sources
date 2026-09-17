using EventsCore.World;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace EventsCore.GameConditions
{
    public class GameCondition_DeathOfDistantStar : GameCondition
    {
        public override void Init()
        {
            base.Init();

            var layer = Find.World.renderer.GetLayer<WorldDrawLayer_DeadStars>(Find.WorldGrid.Surface);
            layer.RegenerateNow();
        }

        public override void End()
        {
            base.End();

            var layer = Find.World.renderer.GetLayer<WorldDrawLayer_DeadStars>(Find.WorldGrid.Surface);
            layer.RegenerateNow();
        }
    }
}
