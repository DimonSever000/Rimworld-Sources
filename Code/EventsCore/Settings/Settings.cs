using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EventsCore.Settings
{
    public abstract class Settings : IExposable
    {
        public Settings()
        {

        }

        public abstract void ResetSettings();
        public virtual void CheckSettings()
        {

        }
        public virtual void ExposeData()
        {

        }
    }
}
