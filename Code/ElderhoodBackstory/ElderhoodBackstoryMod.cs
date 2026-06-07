using ElderhoodBackstory.Patches;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ElderhoodBackstory
{
    public class ElderhoodBackstoryMod : Mod
    {
        private static Harmony harmonyInstance;
        public static Harmony HarmonyInstance => harmonyInstance;

        public ElderhoodBackstoryMod(ModContentPack content) : base(content)
        {
            harmonyInstance = new Harmony("dimonsever000.elderhoodbackstory");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

            MethodInfo method = typeof(Pawn)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
            .FirstOrDefault(m =>
                m.Name.StartsWith("<GetDisabledWorkTypes>g__FillList|") &&
                m.Name.EndsWith("_0") &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType == typeof(List<WorkTypeDef>));

            if (method != null)
            {
                harmonyInstance.Patch(method,
                    postfix: new HarmonyMethod(typeof(Pawn_GetDisabledWorkTypes_FillList_ElderhoodBackstoryPatch)
                        .GetMethod("Postfix", BindingFlags.Static | BindingFlags.NonPublic)));
            }
            else
            {
                Log.Error("[ElderhoodBackstory] error while patch FillList.");
            }

            //MethodInfo method = typeof(Pawn).GetMethod("<GetDisabledWorkTypes>g__FillList|362_0", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            //harmonyInstance.Patch(method, postfix: new HarmonyMethod(typeof(Pawn_GetDisabledWorkTypes_FillList_ElderhoodBackstoryPatch).GetMethod($"Postfix",
            //    BindingFlags.Static | BindingFlags.NonPublic)));
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {

        }
        public override string SettingsCategory()
        {
            return "ElderhoodBackstory.ElderhoodBackstoryMod".Translate();
        }
    }
}
