using HarmonyLib;
using RimWorld;

namespace VAspirE;

public static class GrowthMomentPatches
{
    private static readonly int[] aspirationGainCountPerGrowthTier =
    {
        1,
        1,
        2,
        2,
        3,
        3,
        4,
        4,
        4
    };

    private static readonly int[] aspirationChoiceCountPerGrowthTier =
    {
        2,
        3,
        3,
        4,
        4,
        5,
        5,
        6,
        6
    };

    public static void Apply(Harmony harm)
    {
        harm.Patch(AccessTools.Method(typeof(ChoiceLetter_GrowthMoment), nameof(ChoiceLetter_GrowthMoment.ConfigureGrowthLetter)),
            postfix: new(typeof(GrowthMomentPatches), nameof(AlsoConfigureAspirations)));
    }

    public static void AlsoConfigureAspirations(ChoiceLetter_GrowthMoment __instance)
    {
        if (__instance is ChoiceLetter_GrowthMoment_Aspirations letter)
        {
            var aspirationGainCount = aspirationGainCountPerGrowthTier[__instance.growthTier];
            var aspirationChoiceCount = aspirationChoiceCountPerGrowthTier[__instance.growthTier];

            letter.ConfigureChoiceLetterAspirations(aspirationChoiceCount, aspirationGainCount);
        }
    }
}
