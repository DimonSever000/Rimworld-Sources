using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace EventsCore.Settings
{
    [StaticConstructorOnStartup]
    public class EventModSettings : ModSettings
    {
        private List<Settings> settings = new List<Settings>();
        private static Dictionary<Type, Settings> settingsDict;

        private static float height;
        private static Vector2 scroll;

        private List<SettingsTabRecord> tabs = new List<SettingsTabRecord>();
        public List<SettingsTabRecord> Tabs => tabs;

        private SettingsTabDef curTabInt;
        public SettingsTabDef CurTab
        {
            get
            {
                return curTabInt;
            }
            set
            {
                if (value != curTabInt)
                {
                    curTabInt = value;
                }
            }
        }
        private SettingsTabRecord CurTabRecord
        {
            get
            {
                var record = tabs.Find(x => x.Selected);
                if (record == null)
                {
                    return tabs.First();
                }
                return record;
            }
        }

        public void CacheSettings()
        {
            settingsDict = new Dictionary<Type, Settings>();
            CheckSettings();
            foreach (Settings settings in settings)
            {
                settingsDict.Add(settings.GetType(), settings);
            }
            Write();
        }
        public void ApplyDefaultSettings()
        {
            CheckSettings();

            foreach (Settings setting in settings)
            {
                setting.ResetSettings();
            }

            Write();

            Messages.Message("EventsCore.Settings.LoadDefaultSettingsMessage".Translate(), MessageTypeDefOf.PositiveEvent);
        }

        public void DoSettingsWindowContents(Rect inRect)
        {
            Rect drawTabsRect = new Rect();

            if (tabs.NullOrEmpty())
            {
                foreach (SettingsTabDef tabDef in DefDatabase<SettingsTabDef>.AllDefs)
                {
                    object[] parms = new object[] {tabDef, tabDef.LabelCap.ToString(), new Action(delegate
                    {
                        CurTab = tabDef;
                    }), new Func<bool>(delegate
                    {
                        return CurTab == tabDef;
                    })};
                    SettingsTabRecord record = Activator.CreateInstance(tabDef.workerClass, parms) as SettingsTabRecord;
                    tabs.Add(record);
                }
            }
            else
            {
                drawTabsRect = new Rect(inRect.x, inRect.y + 40, inRect.width, inRect.height - 45 - 40);
                Widgets.DrawMenuSection(drawTabsRect);
                TabDrawer.DrawTabs(drawTabsRect, tabs);

                Rect scrollVertRect = new Rect(0, 0, drawTabsRect.width - 20, height);
                Widgets.BeginScrollView(drawTabsRect, ref scroll, scrollVertRect);
                height = CurTabRecord.OnGUI(new Rect(drawTabsRect.x, drawTabsRect.y, drawTabsRect.width - 20, drawTabsRect.height));

                Widgets.EndScrollView();
            }

            Rect lowerRect = new Rect(drawTabsRect.x, drawTabsRect.y + drawTabsRect.height + 5, drawTabsRect.width, 40);
            //Widgets.DrawMenuSection(lowerRect);

            Rect loadDefaultRect = new Rect(lowerRect.x, lowerRect.y, 200, lowerRect.height);
            if (Widgets.ButtonText(loadDefaultRect, "EventsCore.Settings.LoadDefaultSettingsButton".Translate()))
            {
                ApplyDefaultSettings();
            }
        }

        public Settings TryGetSettings(Type type)
        {
            if (settingsDict.TryGetValue(type, out var value))
            {
                return value;
            }
            return null;
        }
        public T TryGetSettings<T>() where T : Settings
        {
            return TryGetSettings(typeof(T)) as T;
        }
        private void CheckSettings()
        {
            if (settings.NullOrEmpty())
            {
                settings = new List<Settings>();
            }

            settings.RemoveAll(x => x == null);

            foreach (Type type in typeof(Settings).AllSubclassesNonAbstract())
            {
                if (settings.Find(x => x.GetType() == type) == null)
                {
                    settings.Add((Settings)Activator.CreateInstance(type));
                }
            }

            foreach(Settings settings in settings)
            {
                settings.CheckSettings();
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref settings, "settings", LookMode.Deep);
        }
    }
}
