using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace VAspirE;

public class MapComponent_PawnList : MapComponent
{
   

    public MapComponent_PawnList(Map map)
        : base(map)
    {
    }

    public override void FinalizeInit()
    {
        WorldComponent_PawnList.RefreshNeedList(null);
    }
}
