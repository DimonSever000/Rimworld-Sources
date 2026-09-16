using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Diseases.Patches
{
    [HarmonyPatch(typeof(AgeInjuryUtility))]
    [HarmonyPatch("RandomHediffsToGainOnBirthday")]
    [HarmonyPatch(new Type[] { typeof(ThingDef), typeof(float), typeof(float) })]
    public class AgeInjuryUtility_RandomHediffsToGainOnBirthday_DiseasesPatch
    {
        private static void Postfix(ThingDef raceDef, float cancerFactor, float age, ref IEnumerable<HediffGiver_Birthday> __result)
        {
            if (__result != null)
            {
                __result = __result.Where(x => Utility.Settings.BirthdayDiseaseIsEnabled(x.hediff));
            }
        }
    }
}
