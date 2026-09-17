using EventsCore.Utilities;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace EventsCore.GameConditions
{
    public class GameCondition_PsyStorm : GameCondition
    {
        private static readonly SkyColorSet psyStormColors = new SkyColorSet(new ColorInt(216, 0, 255).ToColor, new ColorInt(234, 200, 255).ToColor, new Color(0.6f, 0.8f, 0.5f), 0.85f);

        private static readonly List<SkyOverlay> overlays = new List<SkyOverlay>
        {
            new WeatherOverlay_Fog()
        };

        public override void GameConditionTick()
        {
            for (int i = 0; i < overlays.Count; i++)
            {
                overlays[i].TickOverlay(SingleMap, 1f);
            }

            if (Find.TickManager.TicksGame % 2500 == 0)
            {
                DoEffect();
            }
        }

        private void DoEffect()
        {
            foreach(Pawn pawn in SingleMap.mapPawns.AllPawnsSpawned)
            {
                if (!pawn.RaceProps.IsFlesh || !pawn.RaceProps.Humanlike || pawn.RaceProps.IsMechanoid || pawn.IsMutant)
                {
                    continue;
                }

                if (pawn.Position.Roofed(pawn.Map))
                {
                    continue;
                }

                AffectPawn(pawn);
            }
        }

        private void AffectPawn(Pawn pawn)
        {
            TryMentalBreak(pawn);
            TryStartInspiration(pawn);
            TryChangePassion(pawn);
            TryGainPsylink(pawn);
        }

        private void TryMentalBreak(Pawn pawn)
        {
            float change = 0.03f * pawn.GetStatValue(StatDefOf.PsychicSensitivity);

            if (Rand.Chance(change))
            {
                MentalStateUtility.StartMentalState(pawn, MentalStateDefOf.Berserk);
            }
        }

        private void TryStartInspiration(Pawn pawn)
        {
            if (!pawn.IsPlayerControlled)
            {
                return;
            }

            InspirationHandler inspirationHandler = pawn.mindState?.inspirationHandler;
            if (inspirationHandler == null)
            {
                return;
            }

            float change = 0.015f * pawn.GetStatValue(StatDefOf.PsychicSensitivity);

            if (Rand.Chance(change))
            {
                InspirationDef inspirationDef = inspirationHandler.GetRandomAvailableInspirationDef();
                inspirationHandler.TryStartInspiration(inspirationDef, LabelCap, true);
            }
        }

        private void TryChangePassion(Pawn pawn)
        {
            if (!pawn.IsPlayerControlled)
            {
                return;
            }

            Pawn_SkillTracker skillTracker = pawn.skills;
            if (skillTracker == null || skillTracker.skills == null)
            {
                return;
            }

            float change = 0.005f * pawn.GetStatValue(StatDefOf.PsychicSensitivity);

            if (Rand.Chance(change))
            {
                if (skillTracker.skills.Where(x => !x.TotallyDisabled).TryRandomElementByWeight(x => x.Level, out SkillRecord skillRecord))
                {
                    if (Enum.GetValues(typeof(Passion)).Cast<Passion>().Where(x => x != skillRecord.passion).TryRandomElement(out Passion newPassion))
                    {
                        string letterLabel = "EventsCore.GameCondition_PsyStorm.ChangePassionLabel".Translate(pawn.LabelShort);
                        string letterText = "EventsCore.GameCondition_PsyStorm.ChangePassionDesc".Translate(pawn.LabelShort, skillRecord.def.LabelCap.Colorize(UIUtility.YellowColor),
                            $"Passion{skillRecord.passion}".Translate().Colorize(UIUtility.YellowColor), $"Passion{newPassion}".Translate().Colorize(UIUtility.YellowColor)).Resolve();

                        skillRecord.passion = newPassion;
                        Find.LetterStack.ReceiveLetter(letterLabel, letterText, LetterDefOf.NeutralEvent, pawn);
                    }
                }
            }
        }

        private void TryGainPsylink(Pawn pawn)
        {
            if (!ModsConfig.RoyaltyActive)
            {
                return;
            }

            if (pawn.HasPsylink)
            {
                return;
            }

            float change = 0.003f * pawn.GetStatValue(StatDefOf.PsychicSensitivity);

            if (Rand.Chance(change))
            {
                bool letter = pawn.IsPlayerControlled;
                pawn.ChangePsylinkLevel(1, letter);
            }
        }

        public override void GameConditionDraw(Map map)
        {
            for (int i = 0; i < overlays.Count; i++)
            {
                overlays[i].DrawOverlay(map);
            }
        }

        public override float SkyTargetLerpFactor(Map map)
        {
            return GameConditionUtility.LerpInOutValue(this, TransitionTicks, 0.5f);
        }

        public override SkyTarget? SkyTarget(Map map)
        {
            return new SkyTarget(0.85f, psyStormColors, 1f, 1f);
        }

        public override List<SkyOverlay> SkyOverlays(Map map)
        {
            return overlays;
        }
    }
}
