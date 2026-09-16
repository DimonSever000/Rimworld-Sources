using Diseases.Hediffs;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Diseases
{
    [StaticConstructorOnStartup]
    public static class Utility
    {
        private static DiseasesMod mod;
        public static DiseasesMod Mod
        {
            get
            {
                if (mod == null)
                {
                    mod = LoadedModManager.GetMod<DiseasesMod>();
                }
                return mod;
            }
        }
        private static Settings settings;
        public static Settings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = Mod.GetSettings<Settings>();
                }
                return settings;
            }
        }

        private static HashSet<HediffDef> ageRelatedDiseaseSet;
        public static HashSet<HediffDef> AgeRelatedDiseaseSet => ageRelatedDiseaseSet;
        static Utility()
        {
            ageRelatedDiseaseSet = new HashSet<HediffDef>();
            foreach (HediffDef def in DefDatabase<HediffDef>.AllDefsListForReading)
            {
                if (!HediffGiverSetDefOfLocal.Human.hediffGivers.Any(x => x.hediff == def))
                {
                    continue;
                }

                if (def.modContentPack == Mod.Content)
                {
                    ageRelatedDiseaseSet.Add(def);
                }
            }
        }

        public static bool IsAgeRelatedDisease(this HediffDef def)
        {
            if (ageRelatedDiseaseSet.Contains(def))
            {
                return true;
            }

            return false;
        }
    }
}
