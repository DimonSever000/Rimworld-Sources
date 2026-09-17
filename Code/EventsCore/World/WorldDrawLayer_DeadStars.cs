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
    public class WorldDrawLayer_DeadStars : WorldDrawLayer
    {
        public override Vector3 Position
        {
            get
            {
                if (planetLayer == null)
                {
                    planetLayer = Find.WorldGrid.Surface;
                }

                return base.Position;
            }
        }

        private bool calculatedForStaticRotation;

        private int calculatedForStartingTile = -1;

        private static readonly FloatRange StarsDrawSize = new FloatRange(0.3f, 0.5f);
        protected override int RenderLayer => WorldCameraManager.WorldSkyboxLayer;

        public override bool ShouldRegenerate
        {
            get
            {
                if (!base.ShouldRegenerate && (Find.GameInitData == null || Find.GameInitData.startingTile == calculatedForStartingTile))
                {
                    return UseStaticRotation != calculatedForStaticRotation;
                }
                return true;
            }
        }

        private bool UseStaticRotation => Current.ProgramState == ProgramState.Entry;

        protected override Quaternion Rotation
        {
            get
            {
                if (UseStaticRotation)
                {
                    return Quaternion.identity;
                }
                return Quaternion.LookRotation(GenCelestial.CurSunPositionInWorldSpace());
            }
        }

        public override IEnumerable Regenerate()
        {
            foreach (object item in base.Regenerate())
            {
                yield return item;
            }

            if (Find.World?.GameConditionManager == null)
            {
                yield break;
            }

            foreach (GameCondition gameCondition in Find.World.GameConditionManager.ActiveConditions)
            {
                if (gameCondition.def != GameConditionDefOfLocal.DeathOfDistantStar)
                {
                    continue;
                }

                Rand.PushState(Find.World.info.Seed + gameCondition.uniqueID);

                Vector3 unitVector = Rand.UnitVector3;
                Vector3 pos = unitVector * 10f;
                LayerSubMesh subMesh = GetSubMesh(WorldMaterials.Sun);
                float num = StarsDrawSize.RandomInRange;
                Vector3 rhs = (UseStaticRotation ? GenCelestial.CurSunPositionInWorldSpace().normalized : Vector3.forward);
                float num2 = Vector3.Dot(unitVector, rhs);
                if (num2 > 0.8f)
                {
                    num *= GenMath.LerpDouble(0.8f, 1f, 1f, 0.35f, num2);
                }
                WorldRendererUtility.PrintQuadTangentialToPlanet(pos, num, 0f, subMesh, counterClockwise: true, Rand.Range(0f, 360f));

                Rand.PopState();
            }

            calculatedForStartingTile = ((Find.GameInitData != null) ? Find.GameInitData.startingTile.tileId : (-1));
            calculatedForStaticRotation = UseStaticRotation;
            
            FinalizeMesh(MeshParts.All);
        }
    }
}
