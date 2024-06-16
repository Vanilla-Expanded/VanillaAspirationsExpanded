using HarmonyLib;
using RimWorld;
using Verse;


namespace VAspirE
{

    [HarmonyPatch(typeof(CompAbilityEffect_FleshbeastFromCorpse))]
    [HarmonyPatch("Apply")]
    public static class VAspirE_CompAbilityEffect_FleshbeastFromCorpse_Apply_Patch
    {
      

        [HarmonyPostfix]
        public static void GiveThought(LocalTargetInfo target)
        {
            Corpse corpse;
            if ((corpse = target.Thing as Corpse) != null && corpse.InnerPawn != null)
            {
                foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists_NoSlaves)
                {

                   pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(InternalDefOf.VAE_WitnessedFleshShaping);

                }
            }

            
        }


    }













}

