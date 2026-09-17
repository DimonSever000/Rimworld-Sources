using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Verse;

namespace EventsCore.Settings
{
    public class SettingsTabRecord_World : SettingsTabRecord
    {
        private WorldSettings settings => EventsCoreMod.Settings.TryGetSettings<WorldSettings>();
        public SettingsTabRecord_World(SettingsTabDef def, string label, Action clickedAction, Func<bool> selected)
             : base(def, label, clickedAction, selected)
        {
        }
        public override float OnGUI(Rect rect)
        {
            float localHeight = 0f;

            Rect volcanoesEruptionFactor = new Rect(0, localHeight, rect.width - 20, HorizontalSliderHeight);
            volcanoesEruptionFactor.y = localHeight;
            settings.volcanoesEruptionFactor = Widgets.HorizontalSlider(volcanoesEruptionFactor, settings.volcanoesEruptionFactor, 0.1f, 10f, false, "EventsCore.Settings.VolcanoesEruptionFactor"
                .Translate(settings.volcanoesEruptionFactor.ToStringPercent()));
            localHeight += volcanoesEruptionFactor.height;

            Rect climateChangeRate = volcanoesEruptionFactor;
            climateChangeRate.y = localHeight;
            settings.climateChangeRate = Widgets.HorizontalSlider(climateChangeRate, settings.climateChangeRate, 0.1f, 5f, false, "EventsCore.Settings.ClimateChangeRate"
                .Translate(settings.climateChangeRate.ToStringPercent()));
            localHeight += climateChangeRate.height;

            Rect aiWorldBuildngSize = climateChangeRate;
            aiWorldBuildngSize.y = localHeight;
            settings.aiWorldBuildngSize = Widgets.HorizontalSlider(aiWorldBuildngSize, settings.aiWorldBuildngSize, 0.1f, 5f, false, "EventsCore.Settings.AIWorldBuildngSize"
                .Translate(settings.aiWorldBuildngSize.ToStringPercent()));
            localHeight += aiWorldBuildngSize.height;

            Rect meteoriteOrePercent = climateChangeRate;
            meteoriteOrePercent.y = localHeight;
            settings.meteoriteOrePercent = Widgets.HorizontalSlider(meteoriteOrePercent, settings.meteoriteOrePercent, 0.01f, 1f, false, "EventsCore.Settings.MeteoriteOrePercent"
                .Translate(settings.meteoriteOrePercent.ToStringPercent()));
            localHeight += meteoriteOrePercent.height;

            Rect riverCoolingTexturing = meteoriteOrePercent;
            riverCoolingTexturing.y = localHeight;
            riverCoolingTexturing.height = CheckboxLabeledHeight;
            Widgets.CheckboxLabeled(riverCoolingTexturing.ContractedBy(10f, 0f), "EventsCore.Settings.RiverCoolingTexturing".Translate(), ref settings.riverCoolingTexturing);
            localHeight += riverCoolingTexturing.height;

            Rect riverWarmingTexturing = riverCoolingTexturing;
            riverWarmingTexturing.y = localHeight;
            Widgets.CheckboxLabeled(riverWarmingTexturing.ContractedBy(10f, 0f), "EventsCore.Settings.RiverWarmingTexturing".Translate(), ref settings.riverWarmingTexturing);
            localHeight += riverWarmingTexturing.height;

            return localHeight;
        }
    }
}
