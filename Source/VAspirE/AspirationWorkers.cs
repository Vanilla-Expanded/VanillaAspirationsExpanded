using System.Linq;
using RimWorld;
using Verse;

namespace VAspirE;

public class AspirationWorker_CanHaveKids : AspirationWorker
{
    public AspirationWorker_CanHaveKids(AspirationDef def) : base(def) { }

    public override bool ValidOn(Pawn pawn) =>
        // TODO: Androids can't have kids
        // TODO: Certain HAR races can't have kids
        base.ValidOn(pawn);
}

public class AspirationWorker_AnimaTreeLink : AspirationWorker_Manual
{
    public AspirationWorker_AnimaTreeLink(AspirationDef def) : base(def) { }

    public override bool ValidOn(Pawn pawn) => MeditationFocusDefOf.Natural.CanPawnUse(pawn) && base.ValidOn(pawn);
}

public class AspirationWorker_BecomeNoble : AspirationWorker
{
    public AspirationWorker_BecomeNoble(AspirationDef def) : base(def) { }

    public override bool ValidOn(Pawn pawn) => pawn.royalty.HasAnyTitleIn(Faction.OfEmpire) && base.ValidOn(pawn);

    public override bool IsCompleted(Pawn pawn) => pawn.royalty.HasAnyTitleIn(Faction.OfEmpire);
}

public class AspirationWorker_WieldBladelink : AspirationWorker
{
    public AspirationWorker_WieldBladelink(AspirationDef def) : base(def) { }

    public override bool IsCompleted(Pawn pawn) => pawn.equipment?.Primary?.TryGetComp<CompBladelinkWeapon>() != null;
}

public class AspirationWorker_GainRole : AspirationWorker
{
    public AspirationWorker_GainRole(AspirationDef def) : base(def) { }

    public override bool IsCompleted(Pawn pawn) => pawn.Ideo?.GetRole(pawn) != null;
}

public class AspirationWorker_GainRoleLeader : AspirationWorker
{
    public AspirationWorker_GainRoleLeader(AspirationDef def) : base(def) { }

    public override bool IsCompleted(Pawn pawn) => pawn.Ideo?.GetRole(pawn)?.def == PreceptDefOf.IdeoRole_Leader;
}

public class AspirationWorker_GetBeautifulLover : AspirationWorker
{
    public AspirationWorker_GetBeautifulLover(AspirationDef def) : base(def) { }

    public override bool IsCompleted(Pawn pawn) => pawn.GetLoveCluster().Any(p => p.story?.traits?.GetTrait(TraitDefOf.Beauty) is { Degree: >= 1 });
}

public class AspirationWorker_GrowOld : AspirationWorker
{
    public AspirationWorker_GrowOld(AspirationDef def) : base(def) { }

    public override bool IsCompleted(Pawn pawn) =>
        pawn.ageTracker.AgeBiologicalYears > 65 || Find.LetterStack.letters.Any(letter => letter.label == "LetterLabelBirthday"
           .Translate() && letter.lookTargets.targets.Any(target => target.Thing == pawn));
}

public class AspirationWorker_ObtainLegendaryGear : AspirationWorker
{
    public AspirationWorker_ObtainLegendaryGear(AspirationDef def) : base(def) { }

    public override bool IsCompleted(Pawn pawn)
    {
        foreach (var thing in pawn.equipment.AllEquipmentListForReading.Concat(pawn.apparel.WornApparel.Cast<Thing>()))
            if (thing.TryGetQuality(out var qc) && qc == QualityCategory.Legendary)
                return true;

        return false;
    }
}

public class AspirationWorker_BecomeLiked : AspirationWorker
{
    public AspirationWorker_BecomeLiked(AspirationDef def) : base(def) { }

    public override bool IsCompleted(Pawn pawn)
    {
        var count = 0;
        foreach (var otherPawn in pawn.relations.RelatedPawns.Concat(SocialCardUtility.PawnsForSocialInfo(pawn)))
            if (otherPawn.relations.OpinionOf(pawn) > 0)
                count++;

        return count >= 10;
    }
}

public class AspirationWorker_BecomeDisliked : AspirationWorker
{
    public AspirationWorker_BecomeDisliked(AspirationDef def) : base(def) { }

    public override bool IsCompleted(Pawn pawn)
    {
        var count = 0;
        foreach (var otherPawn in pawn.relations.RelatedPawns.Concat(SocialCardUtility.PawnsForSocialInfo(pawn)))
            if (otherPawn.relations.OpinionOf(pawn) < 0)
                count++;

        return count >= 10;
    }
}
