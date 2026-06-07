using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Noise;
using static RimWorld.PsychicRitualRoleDef;

namespace ScienceRework.Rituals
{
    public class RitualOutcomeEffectWorker_LessonForEducation : RitualOutcomeEffectWorker_FromQuality
    {
        public RitualOutcomeEffectWorker_LessonForEducation()
        {
        }

        public RitualOutcomeEffectWorker_LessonForEducation(RitualOutcomeEffectDef def) : base(def)
        {
        }

        public override void Apply(float progress, Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual)
        {
            float quality = GetQuality(jobRitual, progress);
            RitualOutcomePossibility outcome = GetOutcome(quality, jobRitual);
            LookTargets letterLookTargets = jobRitual.selectedTarget;
            string extraLetterText = null;

            if (jobRitual.Ritual != null)
            {
                ApplyAttachableOutcome(totalPresence, jobRitual, outcome, out extraLetterText, ref letterLookTargets);
            }

            if (outcome.Positive)
            {
                TryLearnForEducation(jobRitual, Utility.BasicLearnXpPerTick * GenDate.TicksPerDay * 0.5f * quality);
            }

            TaggedString text = outcome.description.Formatted(jobRitual.Ritual.Label).CapitalizeFirst();

            string outcomeMoodBreakdown = def.OutcomeMoodBreakdown(outcome);
            if (!outcomeMoodBreakdown.NullOrEmpty())
            {
                text += "\n\n" + outcomeMoodBreakdown;
            }

            if (extraLetterText != null)
            {
                text += "\n\n" + extraLetterText;
            }

            text += "\n\n" + OutcomeQualityBreakdownDesc(quality, progress, jobRitual);

            ApplyDevelopmentPoints(jobRitual.Ritual, outcome, out var extraOutcomeDesc);

            if (extraOutcomeDesc != null)
            {
                text += "\n\n" + extraOutcomeDesc;
            }

            foreach (KeyValuePair<Pawn, int> item in totalPresence)
            {
                if (!outcome.roleIdsNotGainingMemory.NullOrEmpty())
                {
                    RitualRole ritualRole = jobRitual.assignments.RoleForPawn(item.Key);
                    if (ritualRole != null && outcome.roleIdsNotGainingMemory.Contains(ritualRole.id))
                    {
                        continue;
                    }
                }

                if (outcome.memory != null)
                {
                    GiveMemoryToPawn(item.Key, outcome.memory, jobRitual);
                }
            }

            Find.LetterStack.ReceiveLetter("OutcomeLetterLabel".Translate(outcome.label.Named("OUTCOMELABEL"), jobRitual.Ritual.Label.Named("RITUALLABEL")), text, outcome.Positive ? LetterDefOf.RitualOutcomePositive : LetterDefOf.RitualOutcomeNegative, letterLookTargets);
        }

        //public override void Tick(LordJob_Ritual ritual, float progressAmount = 1)
        //{
        //    base.Tick(ritual, progressAmount);

        //    if (Find.TickManager.TicksGame % 900 == 0)
        //    {
        //        TryLearnForEducation(ritual, 0.005f);
        //    }
        //}

        private void TryLearnForEducation(LordJob_Ritual ritual, float amount)
        {
            Pawn teacher = ritual.PawnWithRole("teacher");
            RitualRole role = ritual.assignments.GetRole("student");
            List<Pawn> students = ritual.assignments.AssignedPawns(role).ToList();

            foreach (Pawn student in students)
            {
                student.TryLearnForEducation(teacher, amount);
            }
        }
    }
}
