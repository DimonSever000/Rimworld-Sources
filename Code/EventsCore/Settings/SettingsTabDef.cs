using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EventsCore.Settings
{
    public class SettingsTabDef : Def
    {
        public Type workerClass;

        private SettingsTabRecord workerInt;
        public SettingsTabRecord Worker
        {
            get
            {
                if (workerClass == null)
                {
                    workerInt = (SettingsTabRecord)Activator.CreateInstance(workerClass);
                }
                return workerInt;
            }
        }
    }
}
