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
    public class SettingsTabRecord_IncidentAllowedSettings : SettingsTabRecord
    {
        private IncidentAllowedSettings settings => EventsCoreMod.Settings.TryGetSettings<IncidentAllowedSettings>();
        public SettingsTabRecord_IncidentAllowedSettings(SettingsTabDef def, string label, Action clickedAction, Func<bool> selected)
             : base(def, label, clickedAction, selected)
        {
        }

        public override float OnGUI(Rect rect)
        {
            float localHeight = 0f;

            for(int i = 0; i < settings.ModIncidents.Count; i++)
            {
                IncidentDef def = settings.ModIncidents[i];

                Rect localRect = new Rect(0, localHeight, rect.width, 35f);
                localHeight += localRect.height;

                if (i % 2 == 0)
                {
                    Widgets.DrawLightHighlight(localRect);
                }

                bool value = settings.IsIncidentAllowed(def);
                Widgets.CheckboxLabeled(localRect.ContractedBy(10f, 0f), $"{def.LabelCap}", ref value);
                settings.SetIncidentAllowed(def, value);

                if (Mouse.IsOver(localRect))
                {
                    TaggedString tip = $"{def.LabelCap.Colorize(ColoredText.TipSectionTitleColor)}\n\n{def.letterText}";
                    TooltipHandler.TipRegion(localRect, tip);
                    Widgets.DrawHighlight(localRect);
                }
            }

            return localHeight;
        }
    }
}
