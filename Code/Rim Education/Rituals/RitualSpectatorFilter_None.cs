using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ScienceRework.Rituals
{
    public class RitualSpectatorFilter_None : RitualSpectatorFilter
    {
        public override bool Allowed(Pawn p)
        {
            return false;
        }
    }
}
