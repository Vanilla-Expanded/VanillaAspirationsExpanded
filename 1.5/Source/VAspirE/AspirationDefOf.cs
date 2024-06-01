using RimWorld;
using Verse;

namespace VAspirE;

[DefOf]
public static class AspirationDefOf
{
    [MayRequireRoyalty] public static AspirationDef VAspirE_AnimaTreeLink;
    [MayRequireIdeology] public static AspirationDef VAspirE_ConnectWithGauranlenTree;

    public static AspirationDef VAspirE_GetInspiration;
    public static AspirationDef VAspirE_CreateLegendary;
    public static AspirationDef VAspirE_KillSomeone;

    public static LetterDef VAspirE_Fulfilled;
}
