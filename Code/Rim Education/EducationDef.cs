using JetBrains.Annotations;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ScienceRework
{
    public class EducationDef : Def
    {
        public EducationDef next;

        public EducationDef prev;

        public ExpectationDef minExpectation;

        public DevelopmentalStage developmentalStageFilter = DevelopmentalStage.Newborn | DevelopmentalStage.Baby | DevelopmentalStage.Child | DevelopmentalStage.Adult;

        public TechLevel maxResearchLevel;

        public float difficulty;

        public List<StatModifier> statFactors;

        public List<Aptitude> aptitudes;

        [NoTranslate]
        public string uiIconPath;

        private Texture2D uiIcon;
        public Texture2D UIIcon
        {
            get
            {
                if (uiIcon == null)
                {
                    uiIcon = ContentFinder<Texture2D>.Get(uiIconPath);
                }
                return uiIcon;
            }
        }

        public TaggedString LabelCapFull => $"ScienceRework.Education.Label".Translate(LabelCap);

        public TaggedString DescriptionDetailed(Pawn forPawn)
        {
            CompPawnEducation comp = forPawn.GetComp<CompPawnEducation>();

            if (comp == null)
            {
                Log.ErrorOnce($"No education comp for {forPawn}", forPawn.thingIDNumber);
                return TaggedString.Empty;
            }

            StringBuilder stringBuilder = new StringBuilder();

            if (next != null)
            {
                stringBuilder.Append("ScienceRework.Education.Progress".Translate(next.label.Colorize(ColoredText.TipSectionTitleColor), comp.EducationProgress.ToStringPercent()).Resolve());
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            stringBuilder.Append(description.Formatted(forPawn.Named("PAWN")));

            if (Utility.Settings.educationRestrictionsForResearch)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append("ScienceRework.Education.MaxResearchLevel".Translate(maxResearchLevel.ToStringHuman().Colorize(ColoredText.GeneColor)).Resolve());
            }

            if (Utility.Settings.educationExpectations && minExpectation != null)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append("ScienceRework.Education.Expectations".Translate(forPawn.Named("PAWN"), minExpectation.label.Colorize(ColoredText.GeneColor)).Resolve());
            }

            if (Utility.Settings.educationStats && !statFactors.NullOrEmpty())
            {
                stringBuilder.AppendLine();

                foreach (StatModifier sm in statFactors)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.Append($"    {sm.stat.LabelCap} {sm.ToStringAsFactor}");
                }
            }

            if (Utility.Settings.educationAptitudes && !aptitudes.NullOrEmpty())
            {
                stringBuilder.AppendLine();

                foreach (Aptitude aptitude in aptitudes)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.Append($"    {aptitude.skill.LabelCap} {aptitude.level.ToStringWithSign()}");
                }
            }

            return stringBuilder.ToString().AdjustedFor(forPawn).ResolveTags();
        }

        public float MultiplierOfStat(StatDef stat)
        {
            if (!Utility.Settings.educationStats)
            {
                return 1f;
            }

            float num = 1f;

            if (statFactors != null)
            {
                for (int i = 0; i < statFactors.Count; i++)
                {
                    if (statFactors[i].stat == stat)
                    {
                        num *= statFactors[i].value;
                    }
                }
            }

            return num;
        }

        public int AptitudeFor(SkillDef skill)
        {
            if (!Utility.Settings.educationAptitudes)
            {
                return 0;
            }

            int num = 0;

            if (aptitudes.NullOrEmpty())
            {
                return num;
            }

            for (int i = 0; i < aptitudes.Count; i++)
            {
                if (aptitudes[i].skill == skill)
                {
                    num += aptitudes[i].level;
                }
            }

            return num;
        }

        public bool CanResearch(ResearchProjectDef researchProjectDef)
        {
            if (!Utility.Settings.educationRestrictionsForResearch)
            {
                return true;
            }

            return maxResearchLevel >= researchProjectDef.GetResearchTechLevel();
        }
    }
}
