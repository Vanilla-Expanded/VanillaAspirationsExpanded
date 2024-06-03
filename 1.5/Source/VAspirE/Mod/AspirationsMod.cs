using HarmonyLib;
using RimWorld;
using Verse;

namespace VAspirE;

public class AspirationsMod : Mod
{
    public static Harmony Harm;

    public AspirationsMod(ModContentPack content) : base(content)
    {
        Harm = new("vanillaexpanded.aspirations");

        Harm.Patch(AccessTools.Method(typeof(Pawn_NeedsTracker), "RemoveNeed"), new(GetType(), nameof(NoRemovePermanent)));
        SatisfactionPatches.Apply(Harm);
        GrowthMomentPatches.Apply(Harm);
    }

    public static bool NoRemovePermanent(NeedDef nd) => !nd.HasModExtension<NeedExtension_Permanent>();
}

public class NeedExtension_Permanent : DefModExtension { }
