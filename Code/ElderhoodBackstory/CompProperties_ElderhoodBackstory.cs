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
    public class CompProperties_ElderhoodBackstory : CompProperties
    {
        public int elderhoodAge = 60;
        public CompProperties_ElderhoodBackstory()
        {
            compClass = typeof(CompElderhoodBackstory);
        }
    }
}
