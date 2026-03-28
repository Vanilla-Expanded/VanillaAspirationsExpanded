using RimWorld;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms.Impl;
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

    public static bool WorkTagIsDisabledMinusHediffs(WorkTags w, Pawn p)
    {
        return (CombinedDisabledWorkTagsMinusHediffs(p) & w) != 0;
    }

    public static WorkTags CombinedDisabledWorkTagsMinusHediffs(Pawn p)
    {
       
            WorkTags workTags = p.story?.DisabledWorkTagsBackstoryTraitsAndGenes ?? WorkTags.None;
            workTags |= p.kindDef.disabledWorkTags;
            if (p.royalty != null)
            {
                foreach (RoyalTitle item in p.royalty.AllTitlesForReading)
                {
                    if (item.conceited)
                    {
                        workTags |= item.def.disabledWorkTags;
                    }
                }
            }
            if (ModsConfig.IdeologyActive && p.Ideo != null)
            {
                Precept_Role role = p.Ideo.GetRole(p);
                if (role != null)
                {
                    workTags |= role.def.roleDisabledWorkTags;
                }
            }
            
            foreach (QuestPart_WorkDisabled item2 in QuestUtility.GetWorkDisabledQuestPart(p))
            {
                workTags |= item2.disabledWorkTags;
            }
            if (p.IsMutant)
            {
                workTags |= p.mutant.Def.workDisables;
                if (!p.mutant.IsPassive)
                {
                    workTags &= ~WorkTags.Violent;
                }
            }
            return workTags;
        
    }

}
