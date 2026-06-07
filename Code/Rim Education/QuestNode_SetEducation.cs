using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ScienceRework
{
    public class QuestNode_SetEducation : QuestNode
    {
        public SlateRef<IEnumerable<Pawn>> pawns;

        public SlateRef<EducationDef> education;

        protected override void RunInt()
        {
            Slate slate = QuestGen.slate;
            if (pawns.GetValue(slate) != null)
            {
                foreach(Pawn pawn in pawns.GetValue(slate))
                {
                    pawn.TrySetEducation(education.GetValue(slate));
                }
            }
        }

        protected override bool TestRunInt(Slate slate)
        {
            return true;
        }
    }
}
