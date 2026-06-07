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

namespace ElderhoodBackstory
{
    public class CompElderhoodBackstory : ThingComp
    {
        public CompProperties_ElderhoodBackstory Props => (CompProperties_ElderhoodBackstory)props;

        private BackstoryDef elderhood;
        public BackstoryDef Elderhood
        {
            get
            {
                return elderhood;
            }
            set
            {
                elderhood = value;
            }
        }
        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Defs.Look(ref elderhood, "elderhood");
        }
    }
}
