using LudeonTK;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ScienceRework
{
    public class Settings : ModSettings
    {
        private static Vector2 scroll;
        private static float height;

        public float educationSpeed = 1f;
        public bool educationRestrictionsForResearch = true;
        public bool educationRestrictionsForBooks = true;
        public bool educationByReading = true;
        public bool educationStats = true;
        public bool educationAptitudes = true;
        public bool educationExpectations = true;
        public int anomalyResearchTechLevel = (int)TechLevel.Industrial;

        public void ResetSettings()
        {
            educationSpeed = 1f;
            educationRestrictionsForResearch = true;
            educationRestrictionsForBooks = true;
            educationByReading = true;
            educationStats = true;
            educationAptitudes = true;
            educationExpectations = true;
            anomalyResearchTechLevel = (int)TechLevel.Industrial;
        }

        public void DoSettingsWindowContents(Rect inRect)
        {
            Rect resetSettingsRect = new Rect(inRect.x + inRect.width - 150f, 0, 150f, 40f);
            if (Widgets.ButtonText(resetSettingsRect, "ScienceRework.Settings.ResetSettings".Translate()))
            {
                ResetSettings();
            }

            Widgets.DrawMenuSection(inRect);

            Rect drawRect = new Rect(inRect.x + 20, inRect.y + 20, inRect.width - 25, inRect.height - 40);
            Rect vertRect = new Rect(0, 0, drawRect.width - 20, height);
            Widgets.BeginScrollView(drawRect, ref scroll, vertRect);


            float localHeight = 0f;

            float entryX = 0f;
            float entryWidth = drawRect.width - 20f;
            float entryHeight = 40f;
            Rect entryRect = new Rect(entryX, localHeight, entryWidth, entryHeight);


            educationSpeed = Widgets.HorizontalSlider(new Rect(entryRect.x, entryRect.y + localHeight, entryRect.width, entryRect.height), educationSpeed, 0f, 5f, false, "ScienceRework.Settings.EducationSpeed"
                .Translate(educationSpeed.ToStringPercent()));
            localHeight += entryHeight;


            anomalyResearchTechLevel = (int)Widgets.HorizontalSlider(new Rect(entryRect.x, entryRect.y + localHeight, entryRect.width, entryRect.height), anomalyResearchTechLevel, (int)TechLevel.Undefined, (int)TechLevel.Archotech, false, "ScienceRework.Settings.AnomalyResearchTechLevel"
                .Translate(((TechLevel)anomalyResearchTechLevel).ToStringHuman()));
            localHeight += entryHeight;


            Widgets.CheckboxLabeled(new Rect(entryRect.x, entryRect.y + localHeight, entryRect.width, entryRect.height), "ScienceRework.Settings.EducationRestrictionsForResearch"
                .Translate(educationRestrictionsForResearch ? "Yes".Translate() : "No".Translate()), ref educationRestrictionsForResearch);
            localHeight += entryHeight;


            Widgets.CheckboxLabeled(new Rect(entryRect.x, entryRect.y + localHeight, entryRect.width, entryRect.height), "ScienceRework.Settings.EducationRestrictionsForBooks"
                .Translate(educationRestrictionsForBooks ? "Yes".Translate() : "No".Translate()), ref educationRestrictionsForBooks);
            localHeight += entryHeight;


            Widgets.CheckboxLabeled(new Rect(entryRect.x, entryRect.y + localHeight, entryRect.width, entryRect.height), "ScienceRework.Settings.EducationByReading"
                .Translate(educationByReading ? "Yes".Translate() : "No".Translate()), ref educationByReading);
            localHeight += entryHeight;


            Widgets.CheckboxLabeled(new Rect(entryRect.x, entryRect.y + localHeight, entryRect.width, entryRect.height), "ScienceRework.Settings.EducationStats"
                .Translate(educationStats ? "Yes".Translate() : "No".Translate()), ref educationStats);
            localHeight += entryHeight;


            Widgets.CheckboxLabeled(new Rect(entryRect.x, entryRect.y + localHeight, entryRect.width, entryRect.height), "ScienceRework.Settings.EducationAptitudes"
                .Translate(educationAptitudes ? "Yes".Translate() : "No".Translate()), ref educationAptitudes);
            localHeight += entryHeight;


            Widgets.CheckboxLabeled(new Rect(entryRect.x, entryRect.y + localHeight, entryRect.width, entryRect.height), "ScienceRework.Settings.EducationExpectations"
                .Translate(educationExpectations ? "Yes".Translate() : "No".Translate()), ref educationExpectations);
            localHeight += entryHeight;


            height = localHeight;
            Widgets.EndScrollView();
        }


        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref educationSpeed, "educationSpeed");
            Scribe_Values.Look(ref educationRestrictionsForResearch, "educationRestrictionsForResearch");
            Scribe_Values.Look(ref educationStats, "educationStats");
            Scribe_Values.Look(ref educationExpectations, "educationExpectations");
            Scribe_Values.Look(ref anomalyResearchTechLevel, "anomalyResearchTechLevel");
        }
    }
}
