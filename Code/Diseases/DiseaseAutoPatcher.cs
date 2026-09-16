using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;
using System.Reflection;
using HarmonyLib;

namespace Diseases
{
    [StaticConstructorOnStartup]
    public static class DiseaseAutoPatcher
    {
        static DiseaseAutoPatcher()
        {
            foreach (IncidentDef incidentDef in DefDatabase<IncidentDef>.AllDefsListForReading)
            {
                DefModExtension_DiseaseAutoPatch extension = incidentDef.GetModExtension<DefModExtension_DiseaseAutoPatch>();
                if (extension != null)
                {
                    if (incidentDef.diseaseBiomeRecords == null)
                    {
                        incidentDef.diseaseBiomeRecords = new List<BiomeDiseaseRecord>();
                    }

                    if (incidentDef.diseaseBiomeRecords.Any(x => x.diseaseInc == incidentDef))
                    {
                        continue;
                    }

                    foreach(BiomeDef biomeDef in DefDatabase<BiomeDef>.AllDefsListForReading)
                    {
                        incidentDef.diseaseBiomeRecords.Add(new BiomeDiseaseRecord()
                        {
                            diseaseInc = incidentDef,
                            commonality = 100f
                        });
                    }
                }
            }
        }
    }
}
