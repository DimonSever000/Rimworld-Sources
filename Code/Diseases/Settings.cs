using Diseases.Hediffs;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static UnityEngine.UI.ContentSizeFitter;

namespace Diseases
{
    public class Settings : ModSettings
    {
        private static Vector2 scroll;
        private static float height;

        private bool relatedDiseasesAllowed = true;
        public bool RelatedDiseasesAllowed => relatedDiseasesAllowed;

        private Dictionary<string, bool> ageRelatedDiseasesAllowedDict = new Dictionary<string, bool>();

        public bool BirthdayDiseaseIsEnabled(HediffDef def)
        {
            if (!def.IsAgeRelatedDisease())
            {
                return true;
            }

            if (!ageRelatedDiseasesAllowedDict.TryGetValue(def.defName, out bool result))
            {
                ageRelatedDiseasesAllowedDict.Add(def.defName, true);
                return true;
            }

            return result;
        }

        public void ResetSettings()
        {
            relatedDiseasesAllowed = true;

            ageRelatedDiseasesAllowedDict = new Dictionary<string, bool>();
            foreach (HediffDef def in Utility.AgeRelatedDiseaseSet)
            {
                if (!def.IsAgeRelatedDisease())
                {
                    continue;
                }

                ageRelatedDiseasesAllowedDict.Add(def.defName, true);
            }
        }

        public void DoSettingsWindowContents(Rect inRect)
        {
            Rect resetSettingsRect = new Rect(inRect.x + inRect.width - 150f, 0, 150f, 40f);
            if (Widgets.ButtonText(resetSettingsRect, "Diseases.Settings.ResetSettings".Translate()))
            {
                ResetSettings();
            }

            Widgets.DrawMenuSection(inRect);


            Rect drawRect = new Rect(inRect.x + 20, inRect.y + 20, inRect.width - 25, inRect.height - 40);
            Rect vertRect = new Rect(0, 0, drawRect.width - 20, height);
            Widgets.BeginScrollView(drawRect, ref scroll, vertRect);
            float localHeight = 0f;


            Rect relatedDiseasesRect = new Rect(0, localHeight, drawRect.width - 20, 60);
            Widgets.CheckboxLabeled(relatedDiseasesRect.ContractedBy(10f, 0f), "Diseases.Settings.AllowRelatedDiseases".Translate($"Diseases.{relatedDiseasesAllowed.ToString()}".Translate()), ref relatedDiseasesAllowed);
            localHeight += relatedDiseasesRect.height + 10f;
            if (Mouse.IsOver(relatedDiseasesRect))
            {
                TaggedString tip = $"Diseases.Settings.AllowRelatedDiseasesTip".Translate();
                TooltipHandler.TipRegion(relatedDiseasesRect, tip);
                Widgets.DrawHighlight(relatedDiseasesRect);
            }


            Rect ageRelatedDiseaseRect = new Rect(0, localHeight, drawRect.width - 20, 40);
            Text.Anchor = TextAnchor.MiddleCenter;
            Text.Font = GameFont.Medium;
            Widgets.Label(ageRelatedDiseaseRect, "Diseases.Settings.AgeRelatedDiseases".Translate());
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            localHeight += ageRelatedDiseaseRect.height;
            if (Mouse.IsOver(ageRelatedDiseaseRect))
            {
                TaggedString tip = $"Diseases.Settings.AgeRelatedDiseasesTip".Translate();
                TooltipHandler.TipRegion(ageRelatedDiseaseRect, tip);
                Widgets.DrawHighlight(ageRelatedDiseaseRect);
            }

            int i = 0;
            foreach (HediffDef hediffDef in Utility.AgeRelatedDiseaseSet)
            {
                Rect rect = new Rect(0, localHeight, drawRect.width - 20f, 45f);
                rect.y = localHeight;

                if (i % 2 == 0)
                {
                    Widgets.DrawLightHighlight(rect);
                }

                if (!ageRelatedDiseasesAllowedDict.ContainsKey(hediffDef.defName))
                {
                    ageRelatedDiseasesAllowedDict.Add(hediffDef.defName, true);
                }

                bool value = ageRelatedDiseasesAllowedDict[hediffDef.defName];
                Widgets.CheckboxLabeled(rect.ContractedBy(10f, 0f), $"{hediffDef.LabelCap}", ref value);
                ageRelatedDiseasesAllowedDict[hediffDef.defName] = value;

                if (Mouse.IsOver(rect))
                {
                    TaggedString tip = $"{hediffDef.LabelCap.Colorize(ColoredText.TipSectionTitleColor)}\n\n{hediffDef.description}";
                    TooltipHandler.TipRegion(rect, tip);
                    Widgets.DrawHighlight(rect);
                }

                localHeight += rect.height;
                i++;
            }

            height = localHeight;
            Widgets.EndScrollView();
        }

        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref relatedDiseasesAllowed, "relatedDiseasesAllowed", true);
            Scribe_Collections.Look(ref ageRelatedDiseasesAllowedDict, "ageRelatedDiseasesAllowedDict", LookMode.Value, LookMode.Value);
        }
    }
}
