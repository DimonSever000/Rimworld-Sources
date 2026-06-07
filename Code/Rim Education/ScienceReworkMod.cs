using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ScienceRework
{
    public class ScienceReworkMod : Mod
    {
        private static Harmony harmonyInstance;
        public static Harmony HarmonyInstance => harmonyInstance;

        private Settings settings;

        public ScienceReworkMod(ModContentPack content) : base(content)
        {
            harmonyInstance = new Harmony("dimonsever000.sciencerework");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

            settings = GetSettings<Settings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.DoSettingsWindowContents(inRect);
        }
        public override string SettingsCategory()
        {
            return this.Content.Name;
        }
    }
}
