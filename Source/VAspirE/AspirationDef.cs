using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace VAspirE;

public class AspirationDef : Def
{
    public string iconPath;
    public List<GeneDef> invalidGenes;
    public List<TraitRequirement> invalidTraits;
    public List<XenotypeDef> invalidXenotypes;
    public int minimumAge = -1;
    public List<SkillDef> requiredSkills;
    public TraitDef requiredTrait;
    public List<TraitRequirement> requiredTraitsAny;
    public WorkTags requiredWorkTags = WorkTags.None;
    public bool reverseRelationCheck;
    public HediffDef satisfiedHediff;
    public FloatRange satisfiedHediffSeverityRange = new(float.MinValue, float.MaxValue);
    public PawnRelationDef satisfiedRelation;
    public ThoughtDef satisfiedThought;
    public IntRange satisfiedThoughtDegreeRange = new(int.MinValue, int.MaxValue);
    public List<ThoughtDef> satisfiedThoughtsAny;
    public string satisfiedWhenText;
    public List<XenotypeDef> satisfiedXenotypesAny;
    public Type workerClass = typeof(AspirationWorker);
    private Texture2D icon;
    private AspirationWorker worker;

    public Texture2D Icon => icon ??= ContentFinder<Texture2D>.Get(iconPath);

    public AspirationWorker Worker => worker ??= (AspirationWorker)Activator.CreateInstance(workerClass, this);

    public TaggedString GetSatisfactionText(Pawn pawn)
    {
        if (satisfiedWhenText.NullOrEmpty())
        {
            string type, satisfiedLabel;
            if (satisfiedHediff != null)
            {
                if (typeof(Hediff_Implant).IsAssignableFrom(satisfiedHediff.hediffClass))
                    type = "VAspirE.Implant".Translate();
                else if (typeof(Hediff_AddedPart).IsAssignableFrom(satisfiedHediff.hediffClass))
                    type = "VAspirE.Prosthetic".Translate();
                else
                    type = "VAspirE.Condition".Translate();

                satisfiedLabel = satisfiedHediff.LabelCap;

                if (satisfiedHediffSeverityRange.min > float.MinValue || satisfiedHediffSeverityRange.max < float.MaxValue)
                {
                    var stage = satisfiedHediff.stages.FirstOrDefault(t => t.minSeverity >= satisfiedHediffSeverityRange.min);
                    if (stage != null)
                    {
                        if (stage.overrideLabel != null) satisfiedLabel = stage.overrideLabel;
                        else if (stage.label != null) satisfiedLabel += " (" + stage.label + ")";
                    }
                }
            }
            else if (satisfiedThought != null)
            {
                type = "VAspirE.Thought".Translate();
                satisfiedLabel = satisfiedThought.LabelCap;

                if (satisfiedThoughtDegreeRange.min > int.MinValue || satisfiedThoughtDegreeRange.max < int.MaxValue)
                {
                    var stages = satisfiedThought.stages.GetRange(satisfiedThoughtDegreeRange.min,
                        satisfiedThoughtDegreeRange.max - satisfiedThoughtDegreeRange.min + 1);
                    var stage = stages.FirstOrDefault(s => s.label != null);
                    if (stage != null) satisfiedLabel = stage.LabelCap;
                }
            }
            else if (satisfiedThoughtsAny is { Count: >= 1 })
            {
                type = "VAspirE.Thought".Translate();
                satisfiedLabel = satisfiedThoughtsAny.Select(t => t.LabelCap.Resolve()).Distinct().ToCommaListOr();
            }
            else if (satisfiedRelation != null)
                return "VAspirE.Becomes".Translate(pawn.NameShortColored, satisfiedRelation.GetGenderSpecificLabelCap(pawn));
            else if (satisfiedXenotypesAny is { Count: >= 1 })
            {
                type = "VAspirE.Xenotype".Translate();
                satisfiedLabel = satisfiedXenotypesAny.Select(x => x.LabelCap.Resolve()).Distinct().ToCommaListOr();
            }
            else
            {
                Log.Error($"[VAspirE] Failed to auto-generation satisfiedWhenText for {defName}. Please set the field in your XML.");
                return "ERROR";
            }

            return "VAspirE.Gains".Translate(pawn.NameShortColored, type, satisfiedLabel);
        }

        return satisfiedWhenText.Formatted(pawn.NameShortColored);
    }

    public string TooltipFor(Pawn pawn, int completedTick)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(LabelCap.AsTipTitle());
        stringBuilder.AppendLine();
        stringBuilder.AppendLine(description.Formatted(pawn.NameShortColored));
        stringBuilder.AppendLine();

        if (completedTick == -1)
            stringBuilder.AppendLine(("VAspirE.SatisfiedWhen".Translate() + GetSatisfactionText(pawn)).Colorize(ColoredText.SubtleGrayColor));
        else
        {
            stringBuilder.Append("VAspirE.CompletedOn".Translate());
            stringBuilder.AppendLine(GenDate.DateFullStringWithHourAt(completedTick, Find.WorldGrid.LongLatOf(pawn.Tile)).Colorize(ColoredText.DateTimeColor));
        }

        return stringBuilder.ToString();
    }
}

public class AspirationWorker
{
    public AspirationDef def;

    public AspirationWorker(AspirationDef def) => this.def = def;

    public virtual bool ValidOn(Pawn pawn)
    {
        if (ModsConfig.BiotechActive && !def.invalidXenotypes.NullOrEmpty() && def.invalidXenotypes.Contains(pawn.genes.Xenotype)) return false;

        if (def.requiredTrait != null && !pawn.story.traits.HasTrait(def.requiredTrait)) return false;

        if (!def.requiredTraitsAny.NullOrEmpty() && def.requiredTraitsAny.TrueForAll(trait => !trait.HasTrait(pawn))) return false;

        if (!def.requiredSkills.NullOrEmpty() && def.requiredSkills.Any(skill => pawn.skills.GetSkill(skill).TotallyDisabled)) return false;

        if (def.minimumAge > 0 && pawn.ageTracker.AgeBiologicalYears < def.minimumAge) return false;

        if (!def.invalidTraits.NullOrEmpty() && def.invalidTraits.Any(trait => trait.HasTrait(pawn))) return false;

        if (!def.invalidGenes.NullOrEmpty() && def.invalidGenes.Any(gene => pawn.genes.HasGene(gene))) return false;

        if (def.requiredWorkTags != WorkTags.None && pawn.WorkTagIsDisabled(def.requiredWorkTags)) return false;

        return true;
    }

    public virtual bool IsCompleted(Pawn pawn)
    {
        if (def.satisfiedRelation != null && pawn.relations != null)
            foreach (var otherPawn in pawn.relations.PotentiallyRelatedPawns)
                if (def.reverseRelationCheck)
                {
                    if (def.satisfiedRelation.Worker.InRelation(otherPawn, pawn))
                        return true;
                }
                else
                {
                    if (def.satisfiedRelation.Worker.InRelation(pawn, otherPawn))
                        return true;
                }

        if (def.satisfiedHediff != null && pawn.health?.hediffSet != null)
        {
            var hediffs = new List<Hediff>();
            pawn.health.hediffSet.GetHediffs(ref hediffs, hediff => hediff.def == def.satisfiedHediff);
            foreach (var hediff in hediffs)
                if (def.satisfiedHediffSeverityRange.Includes(hediff.Severity))
                    return true;
        }

        if (ModsConfig.BiotechActive && !def.satisfiedXenotypesAny.NullOrEmpty() && pawn.genes != null)
            foreach (var xenotypeDef in def.satisfiedXenotypesAny)
                if (pawn.genes.Xenotype == xenotypeDef)
                    return true;

        if (def.satisfiedThought != null && pawn.needs?.mood?.thoughts != null)
        {
            if (def.satisfiedThought.IsMemory)
            {
                var memory = pawn.needs.mood.thoughts.memories.GetFirstMemoryOfDef(def.satisfiedThought);
                if (memory != null && def.satisfiedThoughtDegreeRange.Includes(memory.CurStageIndex)) return true;
            }

            if (def.satisfiedThought.IsSituational)
            {
                var state = def.satisfiedThought.Worker.CurrentState(pawn);
                if (state.ActiveFor(def.satisfiedThought) && def.satisfiedThoughtDegreeRange.Includes(state.StageIndexFor(def.satisfiedThought))) return true;
            }
        }

        if (!def.satisfiedThoughtsAny.NullOrEmpty() && pawn.needs?.mood?.thoughts != null)
            foreach (var thoughtDef in def.satisfiedThoughtsAny)
            {
                if (thoughtDef.IsMemory && pawn.needs.mood.thoughts.memories.GetFirstMemoryOfDef(thoughtDef) != null) return true;

                if (thoughtDef.IsSituational && thoughtDef.Worker.CurrentState(pawn).ActiveFor(thoughtDef)) return true;
            }

        return false;
    }
}

public class AspirationWorker_Manual : AspirationWorker
{
    public AspirationWorker_Manual(AspirationDef def) : base(def) { }

    public override bool IsCompleted(Pawn pawn) => false; // This class is for cases where you manually call the Complete method
}
