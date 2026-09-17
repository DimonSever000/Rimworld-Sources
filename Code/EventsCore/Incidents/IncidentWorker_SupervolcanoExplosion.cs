using EventsCore.Utilities;
using EventsCore.World;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Tilemaps;
using UnityEngine.XR;
using Verse;
using Verse.Noise;
using static System.Collections.Specialized.BitVector32;
using static UnityEngine.UI.GridLayoutGroup;

namespace EventsCore.Incidents
{
    public class IncidentWorker_SupervolcanoExplosion : IncidentWorkerWithProps
    {
        public IncidentProperties_SupervolcanoExplosion Props => props as IncidentProperties_SupervolcanoExplosion;

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!ModsConfig.OdysseyActive)
            {
                return false;
            }

            if (base.CanFireNowSub(parms))
            {
                if (VolcanoUtility.TryGetVolcanos(out List<PlanetTile> list))
                {
                    return list.Any();
                }
            }

            return false;
        }
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!ModsConfig.OdysseyActive)
            {
                return false;
            }

            float power = Props.powerRange.RandomInRange;

            if (VolcanoUtility.TrySupervolcanoExplosion(out int tileID, power))
            {
                int ashCount = (int)Props.ashByPowerCurve.Evaluate(power);
                int range = Props.explodeRadius.RandomInRange;

                SendStandardLetter(def.letterLabel, def.letterText, def.letterDef, parms, new LookTargets(tileID), $"{(int)power}".Colorize(UIUtility.YellowColor));
                
                if (DebugSettings.godMode)
                {
                    Find.WorldSelector.SelectedTile = tileID;
                    Find.WorldCameraDriver.JumpTo(tileID);
                }

                return true;
            }

            return false;
        }
    }
}
