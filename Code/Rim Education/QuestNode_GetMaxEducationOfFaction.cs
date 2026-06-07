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
    public class QuestNode_GetMaxEducationOfFaction : QuestNode
    {
        public SlateRef<Faction> faction;

        [NoTranslate]
        public SlateRef<string> storeAs;

        protected override bool TestRunInt(Slate slate)
        {
            DoWork(slate);
            return true;
        }

        protected override void RunInt()
        {
            DoWork(QuestGen.slate);
        }

        private void DoWork(Slate slate)
        {
            Faction f = faction.GetValue(slate);

            if (faction != null)
            {
                DefModExtension_EducationSettings extension = f.def.GetModExtension<DefModExtension_EducationSettings>();

                if (extension?.educationSet != null)
                {
                    if (extension.educationSet.TryGetMaxEducation(out EducationDef education))
                    {
                        slate.Set(storeAs.GetValue(slate), education);
                        return;
                    }
                }
            }

            slate.Set(storeAs.GetValue(slate), EducationDefOfLocal.Uneducated);
        }
    }
}
