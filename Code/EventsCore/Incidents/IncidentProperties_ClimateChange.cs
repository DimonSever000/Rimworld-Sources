using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;
using Verse;
using static HarmonyLib.Code;

namespace EventsCore.Incidents
{
    public class IncidentProperties_ClimateChange : IncidentProperties
    {
        /// <summary>
        /// Отражает изменение температуры за интервал с учетом длительности события. Вход - день от начала события, выход - понижение градусов за интервал
        /// </summary>
        public SimpleCurve temperatureChangePerIntervalByDayPassedCurve;

        /// <summary>
        /// Множитель изменения температуры в зависимости от высоты над уровнем моря
        /// </summary>
        public SimpleCurve elevationTemperatureCurve;

        /// <summary>
        /// Множитель изменения температуры в зависимости от ее расстояния от экватора, где 1 - полюс, 0 - экватор
        /// </summary>
        public SimpleCurve equatorialDistanceTemperatureCurve;


        /// <summary>
        /// Отражает изменение высоты за интервал с учетом длительности события. Вход - день от начала события, выход - метры уровня моря за интервал
        /// </summary>
        public SimpleCurve elevationChangePerIntervalByDayPassedCurve;


        public float targetTemperatureChange;

    }
}
