using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Diseases
{
    public class DiseasesMod : Mod
    {
        private static Harmony harmonyInstance;
        public static Harmony HarmonyInstance => harmonyInstance;

        private Settings settings;

        public DiseasesMod(ModContentPack content) : base(content)
        {
            harmonyInstance = new Harmony("dimonsever000.diseases");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

            settings = GetSettings<Settings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Diseases.DiseasesMod".Translate();
        }
    }
}
