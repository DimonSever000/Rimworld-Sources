using HarmonyLib;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ElderhoodBackstory.Patches
{
    /// <summary>
    /// Нейрослопный транспилер патч на замену префикса
    /// </summary>
    [HarmonyPatch(typeof(CharacterCardUtility), "DoLeftSection")]
    public static class DoLeftSection_ElderhoodPatch
    {
        // Отрисовка Elderhood в нужном месте
        private static void DrawElderhoodAt(Rect sectionRect, Pawn pawn, float num8)
        {
            CompElderhoodBackstory comp = pawn.GetComp<CompElderhoodBackstory>();
            BackstoryDef elderhood = comp?.Elderhood;
            if (elderhood == null)
            {
                return;
            }

            Rect rect7 = new Rect(sectionRect.x, num8, sectionRect.width, 22f);
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect7, "ElderhoodBackstory.Elderhood".Translate());
            Text.Anchor = TextAnchor.UpperLeft;

            string text = elderhood.TitleCapFor(pawn.gender);
            Rect rect8 = new Rect(rect7) { x = rect7.x + 90f, width = Text.CalcSize(text).x + 10f };
            Color color4 = GUI.color;
            GUI.color = CharacterCardUtility.StackElementBackground;
            GUI.DrawTexture(rect8, BaseContent.WhiteTex);
            GUI.color = color4;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect8, text.Truncate(rect8.width));
            Text.Anchor = TextAnchor.UpperLeft;

            if (Mouse.IsOver(rect8))
            {
                Widgets.DrawHighlight(rect8);
                TooltipHandler.TipRegion(rect8, elderhood.FullDescriptionFor(pawn).Resolve());
            }
        }

        // Оборачивает оригинальный drawer секции, добавляя Elderhood после Backstory и Title
        private static Action<Rect> WrapDrawer(Action<Rect> original, Pawn pawn)
        {
            return rect =>
            {
                original(rect); // стандартная отрисовка Childhood/Adulthood + Title

                // Вычисляем вертикальную позицию, на которой остановился оригинальный drawer
                float num8 = rect.y;
                foreach (BackstorySlot slot in Enum.GetValues(typeof(BackstorySlot)))
                {
                    if (pawn.story.GetBackstory(slot) != null)
                    {
                        num8 += 22f + 4f;
                    }
                }
                if (pawn.story?.title != null)
                {
                    num8 += 22f;
                }

                DrawElderhoodAt(rect, pawn, num8);
            };
        }

        // Через рефлексию заменяет drawer в первой секции списка (Backstory)
        private static void ModifyFirstSection(IList list, Pawn pawn)
        {
            if (list.Count == 0)
            {
                return;
            }

            object first = list[0];
            Type type = first.GetType();
            // поле drawer - public
            FieldInfo drawerField = type.GetField("drawer", BindingFlags.Public | BindingFlags.Instance);
            if (drawerField == null)
            {
                return;
            }
            Action<Rect> originalDrawer = (Action<Rect>)drawerField.GetValue(first);
            if (originalDrawer == null)
            {
                return;
            }

            Action<Rect> newDrawer = WrapDrawer(originalDrawer, pawn);
            drawerField.SetValue(first, newDrawer);
            list[0] = first; // записываем структуру обратно (значимый тип!)
        }

        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();

            Type leftRectSectionType = AccessTools.TypeByName("RimWorld.CharacterCardUtility+LeftRectSection");
            Type listType = typeof(List<>).MakeGenericType(leftRectSectionType);
            MethodInfo addMethod = listType.GetMethod("Add", new[] { leftRectSectionType });

            // Ищем локальную переменную, куда сохраняется new List<LeftRectSection>
            int listIndex = -1;
            ConstructorInfo listCtor = listType.GetConstructor(Type.EmptyTypes);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Newobj && codes[i].operand == (object)listCtor)
                {
                    if (i + 1 < codes.Count && codes[i + 1].opcode == OpCodes.Stloc_S)
                    {
                        listIndex = ((LocalBuilder)codes[i + 1].operand).LocalIndex;
                        break;
                    }
                }
            }

            if (listIndex == -1)
            {
                Log.Error("ElderhoodPatch: Cannot find local variable for List<LeftRectSection>");
                return codes;
            }

            MethodInfo modifyMethod = AccessTools.Method(typeof(DoLeftSection_ElderhoodPatch), nameof(ModifyFirstSection));

            // Вставляем вызов ModifyFirstSection после первого добавления Backstory-секции
            bool inserted = false;
            for (int i = 0; i < codes.Count; i++)
            {
                if (!inserted && codes[i].opcode == OpCodes.Callvirt && codes[i].operand == (object)addMethod)
                {
                    // ldloc.s list; ldarg_2 (pawn); call ModifyFirstSection
                    codes.InsertRange(i + 1, new List<CodeInstruction>
                    {
                        new CodeInstruction(OpCodes.Ldloc_S, listIndex),
                        new CodeInstruction(OpCodes.Ldarg_2),
                        new CodeInstruction(OpCodes.Call, modifyMethod)
                    });
                    inserted = true;
                    // Не прерываем цикл, если метод вызывается несколько раз, но мы хотим только первый
                }
            }

            return codes;
        }
    }
}
