using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using UnityEngine.Networking;
using Verse;

namespace VAspirE;

public class AspirationDef : Def
{
    public string iconPath;

    //Invalidations

    public List<GeneDef> invalidGenes;
    public List<TraitRequirement> invalidTraits;
    public List<XenotypeDef> invalidXenotypes;
    public int minimumAge = -1;

    //Requirements

    public List<SkillDef> requiredSkills;
    public List<HediffDef> requiredHediffs;
    public TraitDef requiredTrait;
    public List<TraitRequirement> requiredTraitsAny;
    public WorkTags requiredWorkTags = WorkTags.None;

    //Satisfied by

    public string satisfiedWhenText;

    public HediffDef satisfiedHediff;
    public bool hediffPermanent;
    public int numberOfHediffs = 1;
    public HediffDef satisfiedHediffRemoval;
    public FloatRange satisfiedHediffSeverityRange = new(float.MinValue, float.MaxValue);

    public PawnRelationDef satisfiedRelation;
    public bool reverseRelationCheck;
    public List<ThingDef> thingDefsForRelation;

    public ThoughtDef satisfiedThought;
    public IntRange satisfiedThoughtDegreeRange = new(int.MinValue, int.MaxValue);
    public List<ThoughtDef> satisfiedThoughtsAny;
    
    public List<XenotypeDef> satisfiedXenotypesAny;

    public List<GeneDef> satisfiedGenesAny;

    public RoyalTitleDef satisfiedRoyalTitle;

    public List<AbilityDef> satisfiedAbilitiesAny;
    public int satisfiedAbilityLevel;

    public List<RitualBehaviorDef> satisfiedRitualOutcomesAny;

    public List<ThingDef> satisfiedNuzzledByAny;

    public int satisfiedColonyWealth;

    public RecipeDef satisfiedByRecipe;
    public ThingDef satisfiedByIngredientInRecipe;

    public TraitDef satisfiedTrait;


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
                if (numberOfHediffs>1)
                {
                    satisfiedLabel = numberOfHediffs+ " "+satisfiedHediff.LabelCap;
                }
                if (hediffPermanent)
                {
                    satisfiedLabel += "VAspirE.Scar".Translate();
                }
            }
            else if (satisfiedHediffRemoval !=null)
            {
                return "VAspirE.HediffRemoval".Translate(pawn.NameShortColored, satisfiedHediffRemoval.LabelCap);
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

            else if (satisfiedNuzzledByAny is { Count: >= 1 })
            {

                List<string> names = satisfiedNuzzledByAny.Select(x => x.LabelCap.Resolve()).ToList();
                return "VAspirE.NuzzledBy".Translate(pawn.NameShortColored, names.ToCommaListOr());
            }
            else if (satisfiedColonyWealth > 0)
            {

                return "VAspirE.Wealth".Translate(pawn.NameShortColored, satisfiedColonyWealth);
            }

            else if (satisfiedRelation != null)
            {
                if (thingDefsForRelation.NullOrEmpty())
                {
                    return "VAspirE.Becomes".Translate(pawn.NameShortColored, satisfiedRelation.GetGenderSpecificLabelCap(pawn));
                }
                else
                {
                    List<string> names = thingDefsForRelation.Select(x => x.LabelCap.Resolve()).ToList();
                    return "VAspirE.BecomesList".Translate(pawn.NameShortColored, satisfiedRelation.GetGenderSpecificLabelCap(pawn), names.ToCommaListOr());

                }
            }
            else if (satisfiedAbilityLevel != 0)
            {
                type = "VAspirE.AbilityLevel".Translate();
                satisfiedLabel = satisfiedAbilityLevel.ToString();
            }
            else if (satisfiedByRecipe != null)
            {
                if (satisfiedByIngredientInRecipe != null)
                {
                    return "VAspirE.RecipeProduct".Translate(pawn.NameShortColored, satisfiedByRecipe.LabelCap, satisfiedByIngredientInRecipe.LabelCap);
                }
                else
                {
                    return "VAspirE.Recipe".Translate(pawn.NameShortColored, satisfiedByRecipe.LabelCap);

                }
            }
            else if (satisfiedTrait != null)
            {
                type = "VAspirE.Trait".Translate();
                satisfiedLabel = satisfiedTrait.degreeDatas.First().LabelCap;
            }
           

            else if (satisfiedXenotypesAny is { Count: >= 1 })
            {
                type = "VAspirE.Xenotype".Translate();
                satisfiedLabel = satisfiedXenotypesAny.Select(x => x.LabelCap.Resolve()).Distinct().ToCommaListOr();
            }
            else if (satisfiedAbilitiesAny is { Count: >= 1 })
            {
                type = "VAspirE.Abilities".Translate();
                satisfiedLabel = satisfiedAbilitiesAny.Select(x => x.LabelCap.Resolve()).Distinct().ToCommaListOr();
            }
            else if (satisfiedRoyalTitle != null)
            {
                type = "VAspirE.Title".Translate();
                satisfiedLabel = satisfiedRoyalTitle.LabelCap;
            }
            else if (satisfiedGenesAny is { Count: >= 1 })
            {
                type = "VAspirE.Gene".Translate();
                satisfiedLabel = satisfiedGenesAny.Select(x => x.LabelCap.Resolve()).Distinct().ToCommaListOr();
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
        if(pawn is null) return false;

        if (ModsConfig.BiotechActive && !def.invalidXenotypes.NullOrEmpty() && def.invalidXenotypes.Contains(pawn.genes?.Xenotype)) return false;

        if (def.requiredTrait != null && pawn.story?.traits?.HasTrait(def.requiredTrait)==false) return false;

        if (!def.requiredTraitsAny.NullOrEmpty() && def.requiredTraitsAny.TrueForAll(trait => !trait.HasTrait(pawn))) return false;

        if (!def.requiredSkills.NullOrEmpty() && def.requiredSkills.Any(skill => pawn.skills?.GetSkill(skill)?.TotallyDisabled==true)) return false;

        if (!def.requiredHediffs.NullOrEmpty() && def.requiredHediffs.TrueForAll(hediff => pawn.health?.hediffSet?.HasHediff(hediff)==false)) return false;

        if (def.minimumAge > 0 && pawn.ageTracker?.AgeBiologicalYears < def.minimumAge) return false;

        if (!def.invalidTraits.NullOrEmpty() && def.invalidTraits.Any(trait => trait.HasTrait(pawn))) return false;

        if (!def.invalidGenes.NullOrEmpty() && def.invalidGenes.Any(gene => pawn.genes?.HasActiveGene(gene)==true)) return false;

        if (Current.ProgramState != ProgramState.MapInitializing&&def.requiredWorkTags != WorkTags.None && pawn.WorkTagIsDisabled(def.requiredWorkTags)) return false;

        return true;
    }

    public virtual bool IsCompleted(Pawn pawn)
    {
        if (def.satisfiedRelation != null && pawn.relations != null)
        {

            List<Pawn> listToCheck = new List<Pawn>();
            if (def.thingDefsForRelation.NullOrEmpty())
            {
                listToCheck = pawn.relations.PotentiallyRelatedPawns.ToList();
            }
            else
            {
                listToCheck = pawn.relations.PotentiallyRelatedPawns.Where(x => def.thingDefsForRelation.Contains(x.def)).ToList();
            }
            if (!listToCheck.NullOrEmpty())
            {
                foreach (var otherPawn in listToCheck)
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
            }

        }
        if (def.satisfiedHediff != null && pawn.health?.hediffSet != null)
        {
            if (def.hediffPermanent)
            {
                var hediffs = new List<Hediff>();
                pawn.health.hediffSet.GetHediffs(ref hediffs, hediff => hediff.def == def.satisfiedHediff&&hediff.TryGetComp<HediffComp_GetsPermanent>()?.IsPermanent == true);
                if(hediffs.Count >= def.numberOfHediffs) {
                    return true;
                }
            }
            else
            {
                var hediffs = new List<Hediff>();
                pawn.health.hediffSet.GetHediffs(ref hediffs, hediff => hediff.def == def.satisfiedHediff);
                foreach (var hediff in hediffs)
                    if (def.satisfiedHediffSeverityRange.Includes(hediff.Severity))
                        return true;
            }
           
        }
        if (def.satisfiedHediffRemoval != null && pawn.health?.hediffSet != null)
        {
            if (pawn.health.hediffSet.GetFirstHediffOfDef(def.satisfiedHediffRemoval) == null)
            {
                return true;
            }
        }

        if (ModsConfig.BiotechActive && !def.satisfiedXenotypesAny.NullOrEmpty() && pawn.genes != null)
            foreach (var xenotypeDef in def.satisfiedXenotypesAny)
                if (pawn.genes.Xenotype == xenotypeDef)
                    return true;

        if (!def.satisfiedAbilitiesAny.NullOrEmpty() && pawn.abilities != null)
            foreach (var abilityDef in def.satisfiedAbilitiesAny)
                if (pawn.abilities.GetAbility(abilityDef) != null)
                    return true;

        if (ModsConfig.BiotechActive && !def.satisfiedGenesAny.NullOrEmpty() && pawn.genes != null)
            foreach (var geneDef in def.satisfiedGenesAny)
                if (pawn.genes.HasActiveGene(geneDef))
                    return true;


        if (def.satisfiedByRecipe != null)
        {
            if (StaticCollectionsClass.pawns_and_completed_recipes.ContainsKey(pawn))
            {
                if (def.satisfiedByIngredientInRecipe != null)
                {
                    if (StaticCollectionsClass.pawns_and_completed_recipes[pawn].Item1 == def.satisfiedByRecipe
                        && StaticCollectionsClass.pawns_and_completed_recipes[pawn].Item2?.Where(x => x.def == def.satisfiedByIngredientInRecipe).Count()>0

                        )
                    {
                        return true;
                    }
                }
                else
                {
                    if (StaticCollectionsClass.pawns_and_completed_recipes[pawn].Item1 == def.satisfiedByRecipe)
                    {
                        return true;
                    }
                }
            }

        }

        if (ModsConfig.RoyaltyActive && def.satisfiedRoyalTitle != null)
        {
            if (pawn.royalty.HasAnyTitleIn(Faction.OfEmpire) && (pawn.royalty.HasTitle(def.satisfiedRoyalTitle) ||
                pawn.royalty.highestTitles[Faction.OfEmpire].seniority > def.satisfiedRoyalTitle.seniority))
            {
                return true;
            }
        }
        if (def.satisfiedTrait != null)
        {
            if (pawn.story.traits.HasTrait(def.satisfiedTrait))
            {
                return true;
            }
        }
        if (def.satisfiedColonyWealth > 0)
        {
            if (WealthUtility.PlayerWealth >= def.satisfiedColonyWealth)
            {
                return true;
            }
        }

        if (ModsConfig.RoyaltyActive && def.satisfiedAbilityLevel > 0)
        {
            List<Ability> abilities = pawn?.abilities?.abilities?.ToList();

            if (!abilities.NullOrEmpty() && abilities.Where(x => x.def.level >= def.satisfiedAbilityLevel).Count() > 0)
            {
                return true;
            }

            HediffDef hediffDef = DefDatabase<HediffDef>.GetNamedSilentFail("VPE_PsycastAbilityImplant");
            if (hediffDef != null)
            {
                Hediff_Level hediff = (Hediff_Level)pawn?.health?.hediffSet?.GetFirstHediffOfDef(hediffDef);
                if (hediff != null)
                {
                    if (hediff.level >= def.satisfiedAbilityLevel)
                    {
                        return true;
                    }
                }
            }

        }

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
