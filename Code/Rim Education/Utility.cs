using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace ScienceRework
{
    [StaticConstructorOnStartup]
    public static class Utility
    {
        public const float BasicLearnXpPerTick = 0.5f / GenDate.TicksPerDay;

        private static Settings settings;
        public static Settings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = LoadedModManager.GetMod<ScienceReworkMod>().GetSettings<Settings>();
                }

                return settings;
            }
        }

        private static Dictionary<TechLevel, EducationSet> educationSetForFactionByDefault;
        public static Dictionary<TechLevel, EducationSet> EducationSetForFactionByDefault => educationSetForFactionByDefault;

        private static Dictionary<TechLevel, EducationDef> minEducationForTechLevelDict;
        public static Dictionary<TechLevel, EducationDef> MinEducationForTechLevelDict => minEducationForTechLevelDict;

        static Utility()
        {
            FillAllDataDict();
            PatchAllFactions();
        }

        private static void FillAllDataDict()
        {
            minEducationForTechLevelDict = new Dictionary<TechLevel, EducationDef>();
            foreach (TechLevel techLevel in typeof(TechLevel).GetEnumValues())
            {
                if (TryGetMinEducationForTechLevel(techLevel, out EducationDef education))
                {
                    minEducationForTechLevelDict.Add(techLevel, education);
                }
                else
                {
                    minEducationForTechLevelDict.Add(techLevel, EducationDefOfLocal.Uneducated);
                }
            }

            educationSetForFactionByDefault = new Dictionary<TechLevel, EducationSet>()
            {
                {
                    TechLevel.Undefined,
                    new EducationSet()
                    {
                        educationChances = new List<EducationChance>()
                        {
                            new EducationChance(EducationDefOfLocal.Uneducated, 1f),
                        }
                    }
                },
                {
                    TechLevel.Animal,
                    new EducationSet()
                    {
                        educationChances = new List<EducationChance>()
                        {
                            new EducationChance(EducationDefOfLocal.Uneducated, 1f),
                        }
                    }
                },

                {
                    TechLevel.Neolithic,
                    new EducationSet()
                    {
                        educationChances = new List<EducationChance>()
                        {
                            new EducationChance(EducationDefOfLocal.Uneducated, 0.9f),
                            new EducationChance(EducationDefOfLocal.Primary, 0.1f),
                        }
                    }
                },
                {
                    TechLevel.Medieval,
                    new EducationSet()
                    {
                        educationChances = new List<EducationChance>()
                        {
                            new EducationChance(EducationDefOfLocal.Uneducated, 0.3f),
                            new EducationChance(EducationDefOfLocal.Primary, 0.6f),
                            new EducationChance(EducationDefOfLocal.Basic, 0.1f),
                        }
                    }
                },
                {
                    TechLevel.Industrial,
                    new EducationSet()
                    {
                        educationChances = new List<EducationChance>()
                        {
                            new EducationChance(EducationDefOfLocal.Primary, 0.2f),
                            new EducationChance(EducationDefOfLocal.Basic, 0.65f),
                            new EducationChance(EducationDefOfLocal.Higher, 0.13f),
                            new EducationChance(EducationDefOfLocal.Academic, 0.02f),
                        }
                    }
                },
                {
                    TechLevel.Spacer,
                    new EducationSet()
                    {
                        educationChances = new List<EducationChance>()
                        {
                            new EducationChance(EducationDefOfLocal.Basic, 0.15f),
                            new EducationChance(EducationDefOfLocal.Higher, 0.7f),
                            new EducationChance(EducationDefOfLocal.Academic, 0.1f),
                            new EducationChance(EducationDefOfLocal.Elite, 0.05f),
                        }
                    }
                },
                {
                    TechLevel.Ultra,
                    new EducationSet()
                    {
                        educationChances = new List<EducationChance>()
                        {
                            new EducationChance(EducationDefOfLocal.Basic, 0.05f),
                            new EducationChance(EducationDefOfLocal.Higher, 0.15f),
                            new EducationChance(EducationDefOfLocal.Academic, 0.6f),
                            new EducationChance(EducationDefOfLocal.Elite, 0.2f),
                        }
                    }
                },
                {
                    TechLevel.Archotech,
                    new EducationSet()
                    {
                        educationChances = new List<EducationChance>()
                        {
                            new EducationChance(EducationDefOfLocal.Academic, 0.2f),
                            new EducationChance(EducationDefOfLocal.Elite, 0.8f),
                        }
                    }
                },
            };
        }

        private static void PatchAllFactions()
        {
            foreach (FactionDef factionDef in DefDatabase<FactionDef>.AllDefsListForReading)
            {
                if (factionDef.HasModExtension<DefModExtension_EducationSettings>())
                {
                    continue;
                }

                if (educationSetForFactionByDefault.TryGetValue(factionDef.techLevel, out EducationSet educationSet))
                {
                    DefModExtension_EducationSettings extension = new DefModExtension_EducationSettings();
                    extension.educationSet = educationSet;

                    if (factionDef.modExtensions == null)
                    {
                        factionDef.modExtensions = new List<DefModExtension>();
                    }

                    factionDef.modExtensions.Add(extension);
                }
            }
        }

        public static void DrawEducationPlate(Rect r, EducationDef education, Pawn pawn = null)
        {
            Widgets.DrawHighlightIfMouseover(r);

            Rect rect = new Rect(r.x, r.y, r.width, r.height);

            Rect iconRect = new Rect(r.x + 1f, r.y + 1f, r.height - 2f, r.height - 2f);
            GUI.DrawTexture(iconRect, education.UIIcon);

            Rect labelRect = new Rect(rect.x + rect.height + 5f, rect.y, rect.width - 10f, rect.height);
            Widgets.Label(labelRect, education.LabelCap);

            if (Mouse.IsOver(r))
            {
                TaggedString text = $"{education.LabelCapFull.Colorize(ColoredText.TipSectionTitleColor)}\n{education.DescriptionDetailed(pawn)}";
                TooltipHandler.TipRegion(r, text);
            }
        }

        public static bool ShouldEverHaveEducation(this Pawn pawn)
        {
            CompPawnEducation comp = pawn.GetComp<CompPawnEducation>();
            if (comp == null)
            {
                return false;
            }

            return comp.ShouldEverHaveEducation();
        }

        public static bool TryLearnForEducation(this Pawn pawn, Pawn teacher, float amount)
        {
            CompPawnEducation comp = pawn.GetComp<CompPawnEducation>();

            if (comp != null && comp.Learn(amount, teacher))
            {
                return true;
            }

            return false;
        }

        public static bool TryGetEducation(this Pawn pawn, out EducationDef education)
        {
            CompPawnEducation comp = pawn.GetComp<CompPawnEducation>();

            if (comp != null && comp.TryGetEducation(out education))
            {
                return true;
            }

            education = EducationDefOfLocal.Uneducated;

            return false;
        }

        public static bool TrySetEducation(this Pawn pawn, EducationDef education)
        {
            CompPawnEducation comp = pawn.GetComp<CompPawnEducation>();

            if (comp != null && comp.TrySetEducation(education))
            {
                return true;
            }

            return false;
        }

        public static TechLevel GetResearchTechLevel(this ResearchProjectDef researchProjectDef)
        {
            if (researchProjectDef.knowledgeCategory != null)
            {
                return (TechLevel)Utility.Settings.anomalyResearchTechLevel;
            }

            return researchProjectDef.techLevel;
        }

        public static bool CanResearch(this Pawn pawn, ResearchProjectDef researchProjectDef)
        {
            if (pawn.TryGetEducation(out EducationDef education))
            {
                return education.CanResearch(researchProjectDef);
            }

            return false;
        }

        public static bool TryGetMinEducationForTechLevel(TechLevel techLevel, out EducationDef education)
        {
            if (DefDatabase<EducationDef>.AllDefsListForReading.Where(x => x.maxResearchLevel >= techLevel).TryMinBy(x => x.maxResearchLevel, out education))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Приоритет по возрастанию: Раса -> Фракция -> Титул -> Предыстория -> CreepJoiner -> PawnKind
        /// </summary>
        public static bool TryGenerateEducationFor(Pawn pawn, out EducationDef education)
        {
            education = null;

            if (!pawn.ShouldEverHaveEducation())
            {
                return false;
            }

            if (TryGenerateEducationForPawnKind(pawn, out education) ||
                TryGenerateEducationForBackstories(pawn, out education) ||
                TryGenerateEducationForTitle(pawn, out education) ||
                TryGenerateEducationForFaction(pawn, out education) ||
                TryGenerateEducationForRace(pawn, out education))
            {
                education = ApplyPawnEducationRestrictions(pawn, education);
                return true;
            }

            return false;
        }

        private static EducationDef ApplyPawnEducationRestrictions(Pawn pawn, EducationDef education)
        {
            return ApplyPawnEducationRestrictionsAge(pawn, education);
        }

        private static EducationDef ApplyPawnEducationRestrictionsAge(Pawn pawn, EducationDef education)
        {
            EducationDef current = education;

            int counter = 0;

            while (current != null && counter++ < 50)
            {
                if (current.developmentalStageFilter.Has(pawn.DevelopmentalStage))
                {
                    return current;
                }

                current = current.prev;
            }

            Log.Warning($"Not found valid education for {pawn}. Using {EducationDefOfLocal.Uneducated}");
            return EducationDefOfLocal.Uneducated;
        }

        public static bool TryGenerateEducationForPawnKind(Pawn pawn, out EducationDef education)
        {
            education = null;

            if (pawn.kindDef != null)
            {
                DefModExtension_EducationSettings extension = pawn.kindDef.GetModExtension<DefModExtension_EducationSettings>();

                if (extension?.educationSet != null)
                {
                    if (extension.educationSet.TryGenerateEducation(out education))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool TryGenerateEducationForBackstories(Pawn pawn, out EducationDef education)
        {
            education = null;

            if (pawn.story != null)
            {
                if (!pawn.story.AllBackstories.NullOrEmpty())
                {
                    foreach (BackstoryDef backstory in pawn.story.AllBackstories)
                    {
                        DefModExtension_EducationSettings extension = backstory.GetModExtension<DefModExtension_EducationSettings>();

                        if (extension?.educationSet != null)
                        {
                            if (extension.educationSet.TryGenerateEducation(out EducationDef backstoryEducation))
                            {
                                if (education == null || backstoryEducation.maxResearchLevel > education.maxResearchLevel)
                                {
                                    education = backstoryEducation;
                                }
                            }
                        }
                    }
                }
            }

            return education != null;
        }

        public static bool TryGenerateEducationForTitle(Pawn pawn, out EducationDef education)
        {
            education = null;

            if (pawn.royalty != null)
            {
                RoyalTitle title = pawn.royalty.MostSeniorTitle;

                if (title != null)
                {
                    DefModExtension_EducationSettings extension = title.def.GetModExtension<DefModExtension_EducationSettings>();

                    if (extension?.educationSet != null)
                    {
                        if (extension.educationSet.TryGenerateEducation(out education))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool TryGenerateEducationForFaction(Pawn pawn, out EducationDef education)
        {
            education = null;

            if (pawn.Faction != null)
            {
                DefModExtension_EducationSettings extension = pawn.Faction.def.GetModExtension<DefModExtension_EducationSettings>();

                if (extension?.educationSet != null)
                {
                    if (extension.educationSet.TryGenerateEducation(out education))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool TryGenerateEducationForRace(Pawn pawn, out EducationDef education)
        {
            education = null;

            if (pawn.def != null)
            {
                DefModExtension_EducationSettings extension = pawn.def.GetModExtension<DefModExtension_EducationSettings>();

                if (extension?.educationSet != null)
                {
                    if (extension.educationSet.TryGenerateEducation(out education))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool TryGetCurrentExpectationFor(Pawn pawn, ExpectationDef currentExpectation, out ExpectationDef expectation)
        {
            expectation = null;

            if (Current.ProgramState != ProgramState.Playing)
            {
                return false;
            }

            if (pawn.Faction != Faction.OfPlayer && !pawn.IsPrisonerOfColony)
            {
                return false;
            }

            if (pawn.MapHeld == null)
            {
                return false;
            }

            if (!pawn.TryGetEducation(out EducationDef education))
            {
                return false;
            }

            if (education.minExpectation == null)
            {
                return false;
            }

            if (currentExpectation == null || education.minExpectation.order > currentExpectation.order)
            {
                expectation = education.minExpectation;
                return true;
            }

            return false;
        }
    }
}
