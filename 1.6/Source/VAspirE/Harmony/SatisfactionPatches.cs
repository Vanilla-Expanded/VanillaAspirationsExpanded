using HarmonyLib;
using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;

namespace VAspirE;

public static class SatisfactionPatches
{
    public static void Apply(Harmony harm)
    {
        harm.Patch(AccessTools.Method(typeof(Pawn_RelationsTracker), "GainedOrLostDirectRelation"),
            postfix: new(typeof(SatisfactionPatches), nameof(CheckGeneralDaily)));
        harm.Patch(AccessTools.Method(typeof(HediffSet), nameof(HediffSet.AddDirect)),
            postfix: new(typeof(SatisfactionPatches), nameof(CheckGeneral)));
        harm.Patch(AccessTools.Method(typeof(RitualOutcomeEffectWorker_AnimaTreeLinking), nameof(RitualOutcomeEffectWorker_AnimaTreeLinking.Apply)),
            postfix: new(typeof(SatisfactionPatches), nameof(OnTreeLinkPsychic)));
        harm.Patch(AccessTools.Method(typeof(MemoryThoughtHandler), nameof(MemoryThoughtHandler.TryGainMemory), new[] { typeof(Thought_Memory), typeof(Pawn) }),
            postfix: new(typeof(SatisfactionPatches), nameof(CheckGeneral)));
      harm.Patch(AccessTools.Method(typeof(Pawn_RoyaltyTracker), "OnPostTitleChanged"),
            postfix: new(typeof(SatisfactionPatches), nameof(CheckGeneral)));
        harm.Patch(AccessTools.Method(typeof(Pawn_GeneTracker), nameof(Pawn_GeneTracker.SetXenotypeDirect)),
            postfix: new(typeof(SatisfactionPatches), nameof(CheckGeneral)));
        harm.Patch(AccessTools.Method(typeof(Pawn_GeneTracker), nameof(Pawn_GeneTracker.SetXenotype)),
            postfix: new(typeof(SatisfactionPatches), nameof(CheckGeneral)));
        harm.Patch(AccessTools.Method(typeof(Pawn_EquipmentTracker), nameof(Pawn_EquipmentTracker.AddEquipment)),
            postfix: new(typeof(SatisfactionPatches), nameof(CheckGeneral)));
        foreach (var type in typeof(Precept_Role).AllSubclassesNonAbstract())
            if (type.GetMethods().FirstOrDefault(x => x.Name.Equals(nameof(Precept_Role.Assign)))?.IsDeclaredMember() == true)
                harm.Patch(AccessTools.Method(type, nameof(Precept_Role.Assign)),
                    postfix: new(typeof(SatisfactionPatches), nameof(CheckArgP)));
         harm.Patch(AccessTools.Method(typeof(RitualOutcomeEffectWorker_ConnectToTree), nameof(RitualOutcomeEffectWorker_ConnectToTree.Apply)),
            postfix: new(typeof(SatisfactionPatches), nameof(OnTreeLinkGauranlen)));
        harm.Patch(AccessTools.Method(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.BirthdayBiological)),
            postfix: new(typeof(SatisfactionPatches), nameof(CheckGeneral)));
         harm.Patch(AccessTools.Method(typeof(InspirationHandler), nameof(InspirationHandler.TryStartInspiration)),
            postfix: new(typeof(SatisfactionPatches), nameof(OnInspiration)));
        harm.Patch(AccessTools.Method(typeof(QualityUtility), nameof(QualityUtility.GenerateQualityCreatedByPawn), new[] { typeof(Pawn), typeof(SkillDef), typeof(bool) }),
            postfix: new(typeof(SatisfactionPatches), nameof(CheckQuality)));
        harm.Patch(AccessTools.Method(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Wear)),
            postfix: new(typeof(SatisfactionPatches), nameof(CheckGeneral)));
        harm.Patch(AccessTools.Method(typeof(RecordsUtility), nameof(RecordsUtility.Notify_PawnKilled)),
            postfix: new(typeof(SatisfactionPatches), nameof(OnPawnKilled)));
        harm.Patch(AccessTools.Method(typeof(SituationalThoughtHandler), nameof(SituationalThoughtHandler.UpdateAllMoodThoughts)),
            postfix: new(typeof(SatisfactionPatches), nameof(CheckGeneral)));
        harm.Patch(AccessTools.Method(typeof(Pawn_AbilityTracker), nameof(Pawn_AbilityTracker.GainAbility)),
           postfix: new(typeof(SatisfactionPatches), nameof(CheckGeneral)));
        harm.Patch(AccessTools.Method(typeof(TraitSet), nameof(TraitSet.GainTrait)),
           postfix: new(typeof(SatisfactionPatches), nameof(CheckGeneral)));
        harm.Patch(AccessTools.Method(typeof(RitualBehaviorWorker), nameof(RitualBehaviorWorker.Cleanup)),
           postfix: new(typeof(SatisfactionPatches), nameof(CheckRituals)));
        harm.Patch(AccessTools.Method(typeof(JobDriver_Nuzzle), nameof(JobDriver_Nuzzle.MakeNewToils)),
           postfix: new(typeof(SatisfactionPatches), nameof(CheckNuzzlers)));
        harm.Patch(AccessTools.Method(typeof(RecipeWorker), nameof(RecipeWorker.Notify_IterationCompleted)),
           postfix: new(typeof(SatisfactionPatches), nameof(CheckRecipes)));
        harm.Patch(AccessTools.Method(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.RemoveHediff)),
           postfix: new(typeof(SatisfactionPatches), nameof(CheckGeneral)));
    }

    public static void CheckGeneral(Pawn ___pawn)
    {
        
       
        ___pawn?.needs?.Fulfillment()?.CheckCompletion();
    }

    public static void CheckGeneralDaily(Pawn ___pawn)
    {
        ___pawn?.needs?.Fulfillment()?.CheckCompletion(true);
    }

    public static void OnTreeLinkPsychic(LordJob_Ritual jobRitual)
    {
        var pawn = jobRitual.PawnWithRole("organizer");
        pawn?.needs?.Fulfillment()?.Complete(AspirationDefOf.VAspirE_AnimaTreeLink);
    }

    public static void CheckArgP(Pawn p,bool addThoughts)

    {
        p?.needs?.Fulfillment()?.CheckCompletion();
    }

    public static void OnTreeLinkGauranlen(LordJob_Ritual jobRitual)
    {
        var pawn = jobRitual.PawnWithRole("connector");
        pawn?.needs?.Fulfillment()?.Complete(AspirationDefOf.VAspirE_ConnectWithGauranlenTree);
    }

    public static void OnInspiration(Pawn ___pawn, bool __result)
    {
        if (__result)
            ___pawn?.needs?.Fulfillment()?.Complete(AspirationDefOf.VAspirE_GetInspiration);
    }

    public static void CheckQuality(Pawn pawn, QualityCategory __result)
    {
        if (__result == QualityCategory.Legendary)
            pawn?.needs?.Fulfillment()?.Complete(AspirationDefOf.VAspirE_CreateLegendary);
    }

    public static void OnPawnKilled(Pawn killed, Pawn killer)
    {
        if (killed.RaceProps.Humanlike) killer.needs?.Fulfillment()?.Complete(AspirationDefOf.VAspirE_KillSomeone);
    }

    public static void CheckRituals(LordJob_Ritual ritual, RitualBehaviorWorker __instance)
    {
        List<Pawn> listOfPawns = ritual.PawnsToCountTowardsPresence.ToList();
        StaticCollectionsClass.rituals_and_pawns.Add(__instance.def, listOfPawns);
        foreach(Pawn pawn in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction)
        {
            pawn?.needs?.Fulfillment()?.CheckCompletion();
        }
        StaticCollectionsClass.rituals_and_pawns.Clear();
    }
    public static void CheckNuzzlers(JobDriver_Nuzzle __instance)
    {       
        StaticCollectionsClass.pawns_nuzzled.Add(__instance.TargetA.Pawn,__instance.pawn);
        __instance.TargetA.Pawn?.needs?.Fulfillment()?.CheckCompletion();
        StaticCollectionsClass.pawns_nuzzled.Clear();
    }

    public static void CheckRecipes(RecipeWorker __instance, Pawn billDoer, List<Thing> ingredients)
    {
        if (__instance.recipe != null && billDoer != null)
        {
            StaticCollectionsClass.pawns_and_completed_recipes.Add(billDoer, (__instance.recipe, ingredients));
            billDoer?.needs?.Fulfillment()?.CheckCompletion();
            StaticCollectionsClass.pawns_and_completed_recipes.Clear();
        }
        
    }

}
