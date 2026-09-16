using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Diseases.Hediffs
{
	public class HediffComp_Disease : HediffComp, IExposable
	{
		public virtual HediffCompProperties_Disease Props => (HediffCompProperties_Disease)props;

		public int Tick => Find.TickManager.TicksGame;

		protected int startTick;

		protected int intervalTicks;

		public override void CompPostMake()
		{
			base.CompPostMake();
			UpdateData();

		}
		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
		}

		protected virtual void DoEffect(ref float severityAdjustment)
		{
		}

		protected virtual void UpdateData()
        {
			startTick = Tick;
			intervalTicks = Props.intervalTicks.RandomInRange;
		}

		protected virtual bool IsReady(ref float severityAdjustment)
        {
			return Utility.Settings.RelatedDiseasesAllowed;
        }

		protected virtual bool TryApply(ref float severityAdjustment)
		{
			return true;
		}

		protected virtual void SendLetter(HediffDef hediffDef)
		{
		}


        public void ExposeData()
        {
			CompExposeData();
			Scribe_Values.Look(ref startTick, "startTick");
			Scribe_Values.Look(ref intervalTicks, "intervalTicks");
		}
    }
}
