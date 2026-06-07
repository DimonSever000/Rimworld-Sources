using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ScienceRework.Patches
{
    [HarmonyPatch(typeof(ChoiceLetter_GrowthMoment))]
    [HarmonyPatch("ConfigureGrowthLetter")]
    public static class ChoiceLetter_GrowthMoment_ConfigureGrowthLetter_ScienceReworkPatch
    {
        private static void Postfix(ChoiceLetter_GrowthMoment __instance, Pawn pawn, int passionChoiceCount, int traitChoiceCount, int passionGainsCount, List<string> enabledWorkTypes, Name oldName)
        {
            if (!pawn.TryGetEducation(out EducationDef education))
            {
                return;
            }

            if (pawn.needs?.mood?.thoughts?.memories != null && pawn.relations != null)
            {
                var teachers = pawn.needs.mood.thoughts.memories.Memories
                    .Where(x => x.otherPawn != null && x.def == ThoughtDefOf.WasTaught)
                    .Select(x => x.otherPawn)
                    .Distinct()
                    .OrderByDescending(x => pawn.relations.OpinionOf(x))
                    .Take(5)
                    .ToList();


                string educationText = $"ScienceRework.ChoiceLetter_GrowthMoment_ConfigureGrowthLetter_ScienceReworkPatch.EducationText"
                    .Translate(pawn.LabelShort.Colorize(ColoredText.NameColor), education.label.Colorize(ColoredText.TipSectionTitleColor))
                    .Resolve();

                string teachersText = teachers.NullOrEmpty() ? string.Empty :
                $"ScienceRework.ChoiceLetter_GrowthMoment_ConfigureGrowthLetter_ScienceReworkPatch.SignificantPeopleText"
                    .Translate(string.Join(", ", teachers.Select(x => x.NameShortColored)))
                    .Resolve();

                string text = $"\n\n" + $"{educationText} {teachersText}";

                __instance.text += text;
                __instance.mouseoverText += text;
            }
        }
    }
}
