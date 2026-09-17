using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace EventsCore.Settings
{
    public class EventsCoreMod : Mod
    {
        private static Harmony harmonyInstance;
        public static Harmony HarmonyInstance => harmonyInstance;

        private static EventsCoreMod instance;
        public static EventsCoreMod Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = LoadedModManager.GetMod<EventsCoreMod>();
                }
                return instance;
            }
        }

        private static EventModSettings settings;
        public static EventModSettings Settings => settings;

        public EventsCoreMod(ModContentPack content) : base(content)
        {
            harmonyInstance = new Harmony("dimonsever000.eventscore");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

            settings = GetSettings<EventModSettings>();
            WriteSettings();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.DoSettingsWindowContents(inRect);
        }
        public override string SettingsCategory()
        {
            return "EventsCore.EventsCoreMod".Translate();
        }
    }
}
