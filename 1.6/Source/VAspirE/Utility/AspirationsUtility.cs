using RimWorld;
using System.Collections.Generic;
using Verse;

namespace VAspirE;

public static class AspirationsUtility
{

    public static Need_Fulfillment Fulfillment(this Pawn_NeedsTracker tracker) => tracker.TryGetNeed<Need_Fulfillment>();

    private static List<DirectPawnRelation> tmpLoveRelations = new List<DirectPawnRelation>();

    public static List<DirectPawnRelation> GetLoveRelationsInternal(this Pawn pawn)
    {
        tmpLoveRelations.Clear();
        List<DirectPawnRelation> directRelations = pawn?.relations?.DirectRelations;
        if (!directRelations.NullOrEmpty())
        {
            for (int i = 0; i < directRelations.Count; i++)
            {
                if (LovePartnerRelationUtility.IsLovePartnerRelation(directRelations[i].def))
                {
                    tmpLoveRelations.Add(directRelations[i]);
                }
            }
        }
        
        return tmpLoveRelations;
    }

}
