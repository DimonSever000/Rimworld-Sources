using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ScienceRework.Patches
{
    [HarmonyPatch(typeof(CharacterCardUtility))]
    [HarmonyPatch("DoTopStack")]
    public static class CharacterCardUtility_DoTopStack_ScienceReworkPatch
    {
        private static readonly FieldInfo tmpStackElementsField = AccessTools.Field(typeof(CharacterCardUtility), "tmpStackElements");
        private static Pawn currentPawn;

        private static void AddEducationDrawMethod()
        {
            if (currentPawn.TryGetEducation(out EducationDef education))
            {
                var list = (List<GenUI.AnonymousStackElement>)tmpStackElementsField.GetValue(null);

                if (list != null)
                {
                    list.Add(new GenUI.AnonymousStackElement
                    {
                        drawer = delegate (Rect r)
                        {
                            GUI.color = CharacterCardUtility.StackElementBackground;
                            GUI.DrawTexture(r, BaseContent.WhiteTex);
                            GUI.color = Color.white;
                            Utility.DrawEducationPlate(r, education, currentPawn);
                        },
                        width = Text.CalcSize(education.LabelCap).x + 22f + 15f
                    });
                }
            }
        }

        [HarmonyPrefix]
        private static void Prefix(Pawn pawn)
        {
            currentPawn = pawn;
        }

        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = new List<CodeInstruction>(instructions);
            MethodInfo addMethod = AccessTools.Method(typeof(CharacterCardUtility_DoTopStack_ScienceReworkPatch), nameof(AddEducationDrawMethod));

            int lastDrawCall = -1;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].opcode == OpCodes.Call &&
                    list[i].operand is MethodInfo mi &&
                    mi.Name == "DrawElementStack" &&
                    mi.DeclaringType.Name == "GenUI")
                {
                    lastDrawCall = i;
                    break;
                }
            }

            if (lastDrawCall != -1)
            {
                list.Insert(lastDrawCall, new CodeInstruction(OpCodes.Call, addMethod));
            }

            return list;
        }
    }
}
