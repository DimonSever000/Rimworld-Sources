using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace EventsCore.GameConditions
{
    public class GameCondition_IceStorm : GameCondition
    {
        public override WeatherDef ForcedWeather()
        {
            return WeatherDefOfLocal.SnowHard;
        }
        public override void GameConditionTick()
        {
            if (TicksPassed > 15000)
            {
                if (Find.TickManager.TicksGame % 500 == 0)
                {
                    DoFrostDamage();
                }
            }
        }

        private void DoFrostDamage()
        {
            float damage = Rand.Range(0.4f, 0.6f);
            DamageInfo dinfo = new DamageInfo(DamageDefOf.Cut, damage, 2f);

            for (int i = SingleMap.mapPawns.AllPawnsSpawned.Count -1; i >= 0; i--)
            {
                Pawn pawn = SingleMap.mapPawns.AllPawnsSpawned[i];

                if (!pawn.Position.Roofed(pawn.Map))
                {
                    pawn.TakeDamage(dinfo);
                }
            }
        }

        public override float TemperatureOffset()
        {
            return GameConditionUtility.LerpInOutValue(this, 30000f, -50f);
        }
    }
}
