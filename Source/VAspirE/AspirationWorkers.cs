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

public class AspirationWorker_AnimaTreeLink : AspirationWorker
{
    public AspirationWorker_AnimaTreeLink(AspirationDef def) : base(def) { }

    public override bool ValidOn(Pawn pawn) => MeditationFocusDefOf.Natural.CanPawnUse(pawn) && base.ValidOn(pawn);

    public override bool IsCompleted(Pawn pawn) => false; // Completion is handled by the OnTreeLink patch
}

public class AspirationWorker_BecomeNoble : AspirationWorker
{
    public AspirationWorker_BecomeNoble(AspirationDef def) : base(def) { }

    public override bool ValidOn(Pawn pawn) => pawn.royalty.HasAnyTitleIn(Faction.OfEmpire) && base.ValidOn(pawn);

    public override bool IsCompleted(Pawn pawn) => pawn.royalty.HasAnyTitleIn(Faction.OfEmpire);
}
