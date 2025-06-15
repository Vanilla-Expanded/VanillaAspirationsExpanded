using HarmonyLib;
using LudeonTK;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace VAspirE;

public class WorldComponent_PawnList : WorldComponent
{
  
    public static WorldComponent_PawnList Instance;

    public WorldComponent_PawnList(World world) : base(world) => Instance = this;


    public static List<Pawn> colonists_with_fulfillment_need = new List<Pawn>();

    public static void RefreshNeedList(Pawn pawnJustAdded)
    {
        List<Pawn> pawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists_NoSlaves;
        if(pawnJustAdded != null){ 
            pawns.Add(pawnJustAdded);
        }
        GeneDef gene = DefDatabase<GeneDef>.GetNamedSilentFail("VREA_Power");
        if (gene == null)
        {
            colonists_with_fulfillment_need = pawns;
        }
        else
        {
            foreach (Pawn pawn in pawns)
            {
                if (pawn?.genes?.HasActiveGene(gene)==false)
                {
                    colonists_with_fulfillment_need.Add(pawn);
                }
            }

        }
    }

    public override void FinalizeInit(bool fromLoad)
    {
        RefreshNeedList(null);
    }


}