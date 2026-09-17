using EventsCore.Incidents;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Verse;

namespace EventsCore.Settings
{
    public class IncidentAllowedSettings : Settings
    {
        private Dictionary<string, bool> incidentAllowedDict = new Dictionary<string, bool>();

        private List<IncidentDef> modIncidents;
        public List<IncidentDef> ModIncidents
        {
            get
            {
                if (modIncidents == null)
                {
                    modIncidents = DefDatabase<IncidentDef>.AllDefsListForReading.Where(x => x.modContentPack == EventsCoreMod.Instance.Content).ToList();
                }

                return modIncidents;
            }
        }

        public IncidentAllowedSettings() : base()
        {

        }

        public void SetIncidentAllowed(IncidentDef incidentDef, bool value)
        {
            if (!incidentAllowedDict.ContainsKey(incidentDef.defName))
            {
                return;
            }

            incidentAllowedDict.SetOrAdd(incidentDef.defName, value);
        }

        public bool IsIncidentAllowed(IncidentDef incidentDef)
        {
            if (!incidentAllowedDict.TryGetValue(incidentDef.defName, out bool result))
            {
                return true;
            }

            return result;
        }

        public override void ResetSettings()
        {
            incidentAllowedDict?.Clear();
            incidentAllowedDict = new Dictionary<string, bool>();

            foreach (IncidentDef incidentDef in ModIncidents)
            {
                DefModExtension_IncidentProps extension = incidentDef.GetModExtension<DefModExtension_IncidentProps>();

                if (extension != null)
                {
                    incidentAllowedDict.Add(incidentDef.defName, extension.activeByDefault);
                }
                else
                {
                    incidentAllowedDict.Add(incidentDef.defName, true);
                }
            }
        }

        public override void CheckSettings()
        {
            if (incidentAllowedDict == null)
            {
                incidentAllowedDict = new Dictionary<string, bool>();
            }

            foreach (IncidentDef incidentDef in ModIncidents)
            {
                if (!incidentAllowedDict.ContainsKey(incidentDef.defName))
                {
                    DefModExtension_IncidentProps extension = incidentDef.GetModExtension<DefModExtension_IncidentProps>();

                    if (extension != null)
                    {
                        incidentAllowedDict.Add(incidentDef.defName, extension.activeByDefault);
                    }
                    else
                    {
                        incidentAllowedDict.Add(incidentDef.defName, true);
                    }
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref incidentAllowedDict, "incidentAllowedDict", LookMode.Value, LookMode.Value);
        }
    }
}
