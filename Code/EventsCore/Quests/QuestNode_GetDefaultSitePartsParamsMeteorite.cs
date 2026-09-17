using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace EventsCore.Quests
{
    public class QuestNode_GetDefaultSitePartsParamsMeteorite : QuestNode_GetDefaultSitePartsParams
    {
        protected override bool TestRunInt(Slate slate)
        {
            SetVars(slate);
            return true;
        }

        protected override void RunInt()
        {
            SetVars(QuestGen.slate);
        }

        private void SetVars(Slate slate)
        {
            SiteMakerHelper.GenerateDefaultParams(slate.Get("points", 0f), tile.GetValue(slate), faction.GetValue(slate), sitePartDefs.GetValue(slate), out var sitePartDefsWithParams);
            if (sitePartDefsWithParams != null)
            {
                for (int i = 0; i < sitePartDefsWithParams.Count; i++)
                {
                    if (sitePartDefsWithParams[i].def == SitePartDefOf.PreciousLump)
                    {
                        sitePartDefsWithParams[i].parms.preciousLumpResources = slate.Get<ThingDef>("targetMineable");
                    }
                    if (sitePartDefsWithParams[i].def == SitePartDefOfLocal.WorldMeteorite)
                    {
                        sitePartDefsWithParams[i].parms.preciousLumpResources = slate.Get<ThingDef>("targetMineable");
                    }
                }
            }
            slate.Set(storeSitePartsParamsAs.GetValue(slate), sitePartDefsWithParams);
        }
    }
}
