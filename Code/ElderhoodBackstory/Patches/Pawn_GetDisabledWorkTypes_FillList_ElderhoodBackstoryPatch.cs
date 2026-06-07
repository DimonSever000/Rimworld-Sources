using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using Verse;
using static UnityEngine.ParticleSystem;

namespace ElderhoodBackstory.Patches
{
    public class Pawn_GetDisabledWorkTypes_FillList_ElderhoodBackstoryPatch
    {
        private static void Postfix(ref List<WorkTypeDef> list, ref Pawn __instance)
        {
            if (__instance != null && __instance.story != null && !__instance.IsSlave)
            {
                CompElderhoodBackstory compElderhoodBackstory = __instance.GetComp<CompElderhoodBackstory>();
                if (compElderhoodBackstory != null && compElderhoodBackstory.Elderhood != null && compElderhoodBackstory.Elderhood.DisabledWorkTypes != null)
                {
                    foreach (WorkTypeDef disabledWorkType in compElderhoodBackstory.Elderhood.DisabledWorkTypes)
                    {
                        if (!list.Contains(disabledWorkType))
                        {
                            list.Add(disabledWorkType);
                        }
                    }
                }
            }
        }
    }
}
