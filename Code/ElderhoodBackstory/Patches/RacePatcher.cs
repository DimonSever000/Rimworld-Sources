using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Analytics;
using Verse;
using static UnityEngine.ParticleSystem;
using static Verse.MathEvaluatorCustomFunctions;

namespace ElderhoodBackstory.Patches
{
    [StaticConstructorOnStartup]
    public static class RacePatcher
    {
        static RacePatcher()
        {
            StringBuilder preconfiguredRaces = new StringBuilder($"Preconfigured races:");
            StringBuilder patchedRaces = new StringBuilder($"Patched races:");

            foreach (ThingDef race in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                if (race.race != null && race.race.Humanlike)
                {
                    if (typeof(Corpse).IsAssignableFrom(race.thingClass))
                    {
                        continue;
                    }

                    CompProperties_ElderhoodBackstory props = race.GetCompProperties<CompProperties_ElderhoodBackstory>();

                    if (props == null)
                    {
                        int age = (int)(race.race.lifeExpectancy * 0.75f);

                        if (race.comps == null)
                        {
                            race.comps = new List<CompProperties>();
                        }

                        props = new CompProperties_ElderhoodBackstory()
                        {
                            elderhoodAge = age
                        };

                        race.comps.Add(props);

                        patchedRaces.AppendLine();
                        patchedRaces.Append($"{race}, elderhood age = {props.elderhoodAge}");
                    }
                    else
                    {
                        preconfiguredRaces.AppendLine();
                        preconfiguredRaces.Append($"{race}, elderhood age = {props.elderhoodAge}");
                    }
                }
            }

            if (Prefs.DevMode)
            {
                Log.Message($"[Elderhood Backstory] Race Patcher\n\n{preconfiguredRaces}\n\n----------\n\n{patchedRaces}");
            }
        }
    }
}
