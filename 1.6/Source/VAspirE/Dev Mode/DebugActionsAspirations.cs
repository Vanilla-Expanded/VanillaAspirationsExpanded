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
        DebugActionsUtility.DustPuffFrom(p);
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
        List<DebugActionNode> list =
        [
            new("*Random aspiration", DebugActionType.ToolMapForPawns)
            {
                pawnAction = delegate(Pawn p)
                {
                    var fulfilment = p.needs?.Fulfillment();
                    if (fulfilment == null)
                        return;

                    if (DefDatabase<AspirationDef>.AllDefs
                        .Where(aspirationDef => aspirationDef.Worker.ValidOn(p) && !fulfilment.Aspirations.Contains(aspirationDef) && VanillaAspirationsExpanded_Mod.settings.aspirationStates[aspirationDef.defName])
                        .TryRandomElement(out var aspiration))
                    {
                        fulfilment.DebugAddAspiration(aspiration);
                        DebugActionsUtility.DustPuffFrom(p);
                    }
                },
            }
        ];
        foreach (AspirationDef aspiration in DefDatabase<AspirationDef>.AllDefs)
        {
            list.Add(new DebugActionNode(aspiration.LabelCap, DebugActionType.ToolMapForPawns)
            {
                pawnAction = delegate (Pawn p)
                {
                    p.needs?.Fulfillment()?.DebugAddAspiration(aspiration);
                    DebugActionsUtility.DustPuffFrom(p);
                },               
            });
        }
        return list;
    }

    [DebugAction("Pawns", actionType = DebugActionType.ToolMapForPawns)]
    [UsedImplicitly]
    public static void FulfilAspiration(Pawn p)
    {
        DebugActionsUtility.DustPuffFrom(p);
        var fulfillment = p.needs?.Fulfillment();
        if (fulfillment == null) return;
        if (fulfillment.Aspirations.Count <= fulfillment.NumCompleted) return;
        Find.WindowStack.Add(new Dialog_DebugOptionListLister(Gen.YieldSingle(
                new DebugMenuOption("*Random", DebugMenuOptionMode.Action, () => fulfillment.Complete(fulfillment.Aspirations.Where(aspir => !fulfillment.IsComplete(aspir)).RandomElement())))
            .Concat(fulfillment.Aspirations
            .Where(aspir => !fulfillment.IsComplete(aspir))
            .Select(aspir =>
                new DebugMenuOption(aspir.LabelCap, DebugMenuOptionMode.Action, () => fulfillment.Complete(aspir))))));
    }
}
