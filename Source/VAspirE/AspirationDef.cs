using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace VAspirE;

public class AspirationDef : Def
{
    public string iconPath;
    public List<XenotypeDef> invalidXenotypes;
    public TraitDef requiredTrait;
    public List<TraitRequirement> requiredTraitsAny;
    public bool reverseRelationCheck;
    public HediffDef satisfiedHediff;
    public PawnRelationDef satisfiedRelation;
    public ThoughtDef satisfiedThought;
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
                type = "VAspirE.Implant".Translate();
                satisfiedLabel = satisfiedHediff.LabelCap;
            }
            else if (satisfiedThought != null)
            {
                type = "VAspirE.Thought".Translate();
                satisfiedLabel = satisfiedThought.LabelCap;
            }
            else if (satisfiedRelation != null)
                return "VAspirE.Becomes".Translate(pawn.NameShortColored, satisfiedRelation.GetGenderSpecificLabelCap(pawn));
            else if (satisfiedXenotypesAny != null && satisfiedXenotypesAny.Count >= 1)
            {
                type = "VAspirE.Xenotype".Translate();
                satisfiedLabel = satisfiedXenotypesAny[0].LabelCap;
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
            if (pawn.health.hediffSet.HasHediff(def.satisfiedHediff))
                return true;

        if (ModsConfig.BiotechActive && !def.satisfiedXenotypesAny.NullOrEmpty() && pawn.genes != null)
            foreach (var xenotypeDef in def.satisfiedXenotypesAny)
                if (pawn.genes.Xenotype == xenotypeDef)
                    return true;

        if (def.satisfiedThought != null && pawn.needs?.mood?.thoughts != null)
        {
            if (def.satisfiedThought.IsMemory)
                if (pawn.needs.mood.thoughts.memories.GetFirstMemoryOfDef(def.satisfiedThought) != null)
                    return true;

            if (def.satisfiedThought.IsSituational) Log.Error("[VAspirE] Tying aspirations to situational thoughts is not currently supported.");
        }

        return false;
    }
}
