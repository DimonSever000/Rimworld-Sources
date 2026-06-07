using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static UnityEngine.ParticleSystem;
using static Verse.MathEvaluatorCustomFunctions;

namespace ElderhoodBackstory.Patches
{
    [HarmonyPatch(typeof(Pawn_AgeTracker))]
    [HarmonyPatch("BirthdayBiological")]
    public class Pawn_AgeTracker_BirthdayBiological_ElderhoodBackstoryPatch
    {
        private static void Postfix(ref Pawn ___pawn, int birthdayAge)
        {
            if (___pawn.RaceProps.Humanlike)
            {
                CompElderhoodBackstory compElderhoodBackstory = ___pawn.GetComp<CompElderhoodBackstory>();
                if (compElderhoodBackstory != null && compElderhoodBackstory.Elderhood == null)
                {
                    Utility.FillBackstorySlotDirectlyForPlayer(___pawn);

                    if (birthdayAge >= compElderhoodBackstory.Props.elderhoodAge &&
                        (___pawn.IsColonist || ___pawn.IsPrisonerOfColony || ___pawn.IsSlave))
                    {
                        TaggedString title = compElderhoodBackstory.Elderhood.TitleFor(___pawn.gender).Colorize(ColoredText.TipSectionTitleColor);
                        string desc = compElderhoodBackstory.Elderhood.description.Formatted(___pawn.Named("PAWN")).AdjustedFor(___pawn).Resolve();

                        LetterDef letterDef = ___pawn.IsSlave || ___pawn.IsPrisonerOfColony ? LetterDefOf.NeutralEvent : LetterDefOf.PositiveEvent;
                        Find.LetterStack.ReceiveLetter("ElderhoodBackstory.ElderhoodLetterLabel".Translate(___pawn.LabelShort), 
                            "ElderhoodBackstory.ElderhoodLetterDesc".Translate(___pawn.LabelShort, birthdayAge, title, desc), letterDef);

                        ___pawn.Notify_DisabledWorkTypesChanged();
                    }
                }
            }
        }
    }
}
