using LudeonTK;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ScienceRework
{
    public static class DebugTools
    {
        public const string DebugActionCategory = "Education";

        [DebugAction(DebugActionCategory, "Set Education", false, false, false, false, false, 0, false, allowedGameStates = AllowedGameStates.PlayingOnMap, displayPriority = 1000)]
        private static List<DebugActionNode> SetEducation()
        {
            List<DebugActionNode> list = new List<DebugActionNode>();

            foreach (EducationDef item in DefDatabase<EducationDef>.AllDefsListForReading)
            {
                list.Add(new DebugActionNode(item.LabelCap, DebugActionType.ToolMap, delegate
            {
                IntVec3 center = UI.MouseCell();
                Map currentMap = Find.CurrentMap;

                foreach (Thing thing in currentMap.thingGrid.ThingsAt(center).ToList())
                {
                    if (thing is Pawn pawn)
                    {
                        pawn.TrySetEducation(item);
                    }
                }
            }));
            }

            return list;
        }
    }
}
