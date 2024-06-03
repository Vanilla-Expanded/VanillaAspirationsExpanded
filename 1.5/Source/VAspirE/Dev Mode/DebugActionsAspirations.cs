using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Verse;
using LudeonTK;

namespace VAspirE;

public static class DebugActionsAspirations
{
    [DebugAction("Pawns", actionType = DebugActionType.ToolMapForPawns)]
    [UsedImplicitly]
    public static void RemoveAspiration(Pawn p)
    {
        var fulfillment = p.needs?.Fulfillment();
        if (fulfillment == null) return;
        Find.WindowStack.Add(new Dialog_DebugOptionListLister(fulfillment
           .Aspirations.Select(aspir =>
                new DebugMenuOption(aspir.LabelCap, DebugMenuOptionMode.Action, () => fulfillment.DebugRemoveAspiration(aspir)))));
    }

    [DebugAction("Pawns")]
    [UsedImplicitly]
    public static List<DebugActionNode> AddAspiration()
    {
        return DefDatabase<AspirationDef>.AllDefs.Select(aspir =>
                new DebugActionNode(aspir.LabelCap, DebugActionType.ToolMapForPawns, pawnAction: p => p.needs?.Fulfillment()?.DebugAddAspiration(aspir)))
           .ToList();
    }

//    [DebugAction("Pawns", actionType = DebugActionType.ToolMapForPawns)]
//    [UsedImplicitly]
//    public static void ResetAspirations(Pawn p)
//    {
//        p.needs?.Fulfillment()?.DebugResetCompletion();
//    }
}
