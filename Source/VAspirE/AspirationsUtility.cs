using Prepatcher;
using RimWorld;

namespace VAspirE;

public static class AspirationsUtility
{
    [PrepatcherField]
    public static Need_Fulfillment Fulfillment(this Pawn_NeedsTracker tracker) => tracker.TryGetNeed<Need_Fulfillment>();
}
