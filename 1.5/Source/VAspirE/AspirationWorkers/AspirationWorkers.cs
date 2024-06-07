using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace VAspirE;

public class AspirationWorker_CanHaveKids : AspirationWorker
{
    public AspirationWorker_CanHaveKids(AspirationDef def) : base(def) { }

    public override bool ValidOn(Pawn pawn) {
        if (DefDatabase<GeneDef>.GetNamedSilentFail("VREA_Power") != null)
        {
            return pawn?.genes?.HasActiveGene(DefDatabase<GeneDef>.GetNamed("VREA_Power")) == false;

        }
        return base.ValidOn(pawn);

    } 
}

public class AspirationWorker_AnimaTreeLink : AspirationWorker_Manual
{
    public AspirationWorker_AnimaTreeLink(AspirationDef def) : base(def) { }

    public override bool ValidOn(Pawn pawn) => MeditationFocusDefOf.Natural.CanPawnUse(pawn) && base.ValidOn(pawn);
}

public class AspirationWorker_BecomeNoble : AspirationWorker
{
    public AspirationWorker_BecomeNoble(AspirationDef def) : base(def) { }

    public override bool ValidOn(Pawn pawn) => pawn?.royalty?.HasAnyTitleIn(Faction.OfEmpire)==true && base.ValidOn(pawn);

    public override bool IsCompleted(Pawn pawn) => pawn?.royalty?.HasAnyTitleIn(Faction.OfEmpire) == true;
}

public class AspirationWorker_WieldBladelink : AspirationWorker
{
    public AspirationWorker_WieldBladelink(AspirationDef def) : base(def) { }

    public override bool IsCompleted(Pawn pawn) => pawn?.equipment?.Primary?.TryGetComp<CompBladelinkWeapon>() != null;
}

public class AspirationWorker_GainRole : AspirationWorker
{
    public AspirationWorker_GainRole(AspirationDef def) : base(def) { }

    public override bool IsCompleted(Pawn pawn) => pawn?.Ideo?.GetRole(pawn) != null;
}

public class AspirationWorker_GainRoleLeader : AspirationWorker
{
    public AspirationWorker_GainRoleLeader(AspirationDef def) : base(def) { }

    public override bool IsCompleted(Pawn pawn) => pawn?.Ideo?.GetRole(pawn)?.def == PreceptDefOf.IdeoRole_Leader;
}

public class AspirationWorker_GetBeautifulLover : AspirationWorker
{
    public AspirationWorker_GetBeautifulLover(AspirationDef def) : base(def) { }

    public override bool IsCompleted(Pawn pawn) {
        if (Current.ProgramState == ProgramState.MapInitializing) return false;
        if (pawn != null)
        {
            List<Pawn> loveCluster = pawn.GetLoveRelationsInternal().Select(x => x.otherPawn).ToList();
            if (loveCluster != null && loveCluster.Any(p => p?.story?.traits?.GetTrait(InternalDefOf.Beauty)?.Degree >= 1))
            {
                return true;
            }
        }
        return false;
    } 
}

public class AspirationWorker_GrowOld : AspirationWorker
{
    public AspirationWorker_GrowOld(AspirationDef def) : base(def) { }

    public override bool IsCompleted(Pawn pawn) =>
        pawn?.ageTracker?.AgeBiologicalYears > 65 || Find.LetterStack.letters.Any(letter => letter.label == "LetterLabelBirthday"
           .Translate() && letter.lookTargets.targets.Any(target => target.Thing == pawn));
}

public class AspirationWorker_ObtainLegendaryGear : AspirationWorker
{
    public AspirationWorker_ObtainLegendaryGear(AspirationDef def) : base(def) { }

    public override bool IsCompleted(Pawn pawn)
    {
        if (pawn != null) {
            foreach (var thing in pawn.equipment.AllEquipmentListForReading.Concat(pawn.apparel.WornApparel.Cast<Thing>()))
                if (thing.TryGetQuality(out var qc) && qc == QualityCategory.Legendary)
                    return true;
        }
        

        return false;
    }
}

public class AspirationWorker_BecomeLiked : AspirationWorker
{
    public AspirationWorker_BecomeLiked(AspirationDef def) : base(def) { }

    public override bool IsCompleted(Pawn pawn)
    {
        if (Current.ProgramState == ProgramState.MapInitializing) return false;
        var count = 0;
        if (pawn != null)
        {
            foreach (var otherPawn in pawn.relations?.RelatedPawns?.Concat(SocialCardUtility.PawnsForSocialInfo(pawn)))
                if (otherPawn?.relations?.OpinionOf(pawn) > 0)
                    count++;
        }
        return count >= 10;
    }
}

public class AspirationWorker_BecomeDisliked : AspirationWorker
{
    public AspirationWorker_BecomeDisliked(AspirationDef def) : base(def) { }

    public override bool IsCompleted(Pawn pawn)
    {
        if (Current.ProgramState == ProgramState.MapInitializing) return false;
        var count = 0;
        if (pawn != null)
        {
            foreach (var otherPawn in pawn.relations?.RelatedPawns?.Concat(SocialCardUtility.PawnsForSocialInfo(pawn)))
                if (otherPawn?.relations?.OpinionOf(pawn) < 0)
                    count++;
        }
        return count >= 10;
    }
}


public class AspirationWorker_GetAndroidLover : AspirationWorker
{
    public AspirationWorker_GetAndroidLover(AspirationDef def) : base(def) { }

    public override bool IsCompleted(Pawn pawn) {
        if (Current.ProgramState == ProgramState.MapInitializing) return false;
        return pawn != null && pawn.GetLoveCluster()?.Any(p => p?.genes?.HasActiveGene(DefDatabase<GeneDef>.GetNamed("VREA_Power")) == true) == true;
    } 
    
}

public class AspirationWorker_MaxPain : AspirationWorker
{
    public AspirationWorker_MaxPain(AspirationDef def) : base(def) { }

    public override bool IsCompleted(Pawn pawn)
    {
        if (Current.ProgramState == ProgramState.MapInitializing) return false;
        return pawn?.health?.hediffSet?.PainTotal >= 1;
    }
}

public class AspirationWorker_Nuzzled : AspirationWorker
{
    public AspirationWorker_Nuzzled(AspirationDef def) : base(def) { }

    public override bool IsCompleted(Pawn pawn)
    {
        if (!def.satisfiedNuzzledByAny.NullOrEmpty())
        {
            foreach (var thingDef in def.satisfiedNuzzledByAny)
            {
                if (StaticCollectionsClass.pawns_nuzzled.ContainsKey(pawn))
                {
                    if (StaticCollectionsClass.pawns_nuzzled[pawn].def == thingDef)
                    {
                        return true;
                    }
                }
            }
        }return false;
    }
}

public class AspirationWorker_RitualParticipation : AspirationWorker
{
    public AspirationWorker_RitualParticipation(AspirationDef def) : base(def) { }

    public override bool IsCompleted(Pawn pawn)
    {
        if (!def.satisfiedRitualOutcomesAny.NullOrEmpty())
        {
            foreach (var ritualOutcome in def.satisfiedRitualOutcomesAny)
            {
                if (StaticCollectionsClass.rituals_and_pawns.ContainsKey(ritualOutcome))
                {
                    if (StaticCollectionsClass.rituals_and_pawns[ritualOutcome].Contains(pawn))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
