using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using Verse;

namespace ScienceRework
{
    public class EducationChance
    {
        public EducationDef education;

        public float chance;

        public EducationChance()
        {
        }

        public EducationChance(EducationDef education, float chance)
        {
            this.education = education;
            this.chance = chance;
        }

        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "education", xmlRoot.Name);
            chance = ParseHelper.FromString<float>(xmlRoot.FirstChild.Value);
        }
    }
}
