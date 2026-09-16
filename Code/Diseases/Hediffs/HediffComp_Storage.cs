using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Diseases.Hediffs
{
	public class HediffComp_Storage : HediffComp
	{
		public HediffCompProperties_Storage Props => (HediffCompProperties_Storage)props;

		public List<HediffComp_Disease> comps = new List<HediffComp_Disease>();


		public override void CompPostMake()
		{
			base.CompPostMake();
			InitializeComps();
		}
		private void InitializeComps()
		{
			if (Props.comps == null || Props.comps.Count == 0)
			{
				return;
			}
			foreach(HediffCompProperties hediffCompProperties in Props.comps)
            {
				HediffComp_Disease comp = (HediffComp_Disease)Activator.CreateInstance(hediffCompProperties.compClass);
				if (comp == null)
                {
					continue;
                }
				comp.props = hediffCompProperties;
				comp.parent = this.parent;
				comp.CompPostMake();
				comps.Add(comp);
			}
		}

		public override void CompPostTick(ref float severityAdjustment)
		{
			foreach(HediffComp_Disease comp in comps)
            {
				comp.CompPostTick(ref severityAdjustment);
			}
		}

		public override void CompExposeData()
		{

			base.CompExposeData();
			Scribe_Collections.Look(ref comps, "comps", LookMode.Deep);
			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				for (int i = 0; i < comps.Count; i++)
                {
					comps[i].props = Props.comps[i];
					comps[i].parent = this.parent;
				}
			}
		}

	}
}
