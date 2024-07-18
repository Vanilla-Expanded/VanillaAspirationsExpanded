using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Verse;
using LudeonTK;
using RimWorld;
using UnityEngine.Networking.Types;

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
        List<DebugActionNode> list = new List<DebugActionNode>();
        foreach (AspirationDef aspiration in DefDatabase<AspirationDef>.AllDefs)
        {              
            list.Add(new DebugActionNode(aspiration.defName, DebugActionType.ToolMapForPawns)
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
}
