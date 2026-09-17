using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.AI;

namespace EventsCore.Incidents
{
    public abstract class IncidentWorkerWithProps : IncidentWorker
    {
        private DefModExtension_IncidentProps extension;
        private DefModExtension_IncidentProps Extension
        {
            get
            {
                if (extension == null)
                {
                    extension = def.GetModExtension<DefModExtension_IncidentProps>();
                }
                return extension;
            }
        }

        protected IncidentProperties props => Extension.props;
    }
}
