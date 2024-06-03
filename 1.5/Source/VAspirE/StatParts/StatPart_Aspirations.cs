using RimWorld;
using Verse;

namespace VAspirE;

public class StatPart_Aspirations : StatPart
{
    public float offsetPerAspiration;

    public override void TransformValue(StatRequest req, ref float val)
    {
        if (req.Thing is Pawn pawn && pawn.needs?.TryGetNeed<Need_Fulfillment>() is { } need) val += offsetPerAspiration * need.NumCompleted;
    }

    public override string ExplanationPart(StatRequest req)
    {
        if (req.Thing is Pawn pawn && pawn.needs?.TryGetNeed<Need_Fulfillment>() is { } need)
        {
            var count = need.NumCompleted;
            if (count > 0)
                return "VAspirE.AspirationStat".Translate(count,
                    parentStat.Worker.ValueToString(offsetPerAspiration * count, false, ToStringNumberSense.Offset));
        }

        return "";
    }
}
