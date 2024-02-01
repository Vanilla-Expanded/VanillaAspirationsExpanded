using HarmonyLib;
using RimWorld;
using Verse;

namespace VAspirE;

public static class SatisfactionPatches
{
    public static void Apply(Harmony harm)
    {
        harm.Patch(AccessTools.Method(typeof(Pawn_RelationsTracker), "GainedOrLostDirectRelation"),
            postfix: new(typeof(SatisfactionPatches), nameof(CheckGeneral)));
        harm.Patch(AccessTools.Method(typeof(HediffSet), nameof(HediffSet.AddDirect)),
            postfix: new(typeof(SatisfactionPatches), nameof(CheckGeneral)));
        harm.Patch(AccessTools.Method(typeof(RitualOutcomeEffectWorker_AnimaTreeLinking), nameof(RitualOutcomeEffectWorker_AnimaTreeLinking.Apply)),
            postfix: new(typeof(SatisfactionPatches), nameof(OnTreeLink)));
        harm.Patch(AccessTools.Method(typeof(MemoryThoughtHandler), nameof(MemoryThoughtHandler.TryGainMemory), new[] { typeof(Thought_Memory), typeof(Pawn) }),
            postfix: new(typeof(SatisfactionPatches), nameof(CheckGeneral)));
        harm.Patch(AccessTools.Method(typeof(Pawn_RoyaltyTracker), "OnPostTitleChanged"),
            postfix: new(typeof(SatisfactionPatches), nameof(CheckGeneral)));
        harm.Patch(AccessTools.Method(typeof(Pawn_GeneTracker), nameof(Pawn_GeneTracker.SetXenotypeDirect)),
            postfix: new(typeof(SatisfactionPatches), nameof(CheckGeneral)));
        harm.Patch(AccessTools.Method(typeof(Pawn_GeneTracker), nameof(Pawn_GeneTracker.SetXenotype)),
            postfix: new(typeof(SatisfactionPatches), nameof(CheckGeneral)));
    }

    public static void CheckGeneral(Pawn ___pawn)
    {
        ___pawn.needs?.Fulfillment()?.CheckCompletion();
    }

    public static void OnTreeLink(LordJob_Ritual jobRitual)
    {
        var pawn = jobRitual.PawnWithRole("organizer");
        pawn?.needs?.Fulfillment()?.Complete(AspirationDefOf.VAspirE_AnimaTreeLink);
    }
}
