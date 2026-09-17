using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace EventsCore.Settings
{
    public abstract class SettingsTabRecord : TabRecord
    {
        protected static readonly float HorizontalSliderHeight = 60f;
        protected static readonly float CheckboxLabeledHeight = 35f;

        public readonly SettingsTabDef def;

        public SettingsTabRecord(SettingsTabDef def, string label, Action clickedAction, Func<bool> selected)
            : base(label, clickedAction, selected)
        {
            this.def = def;
        }
        public abstract float OnGUI(Rect rect);
    }
}
