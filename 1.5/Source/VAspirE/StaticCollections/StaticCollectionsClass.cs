
using Verse;
using System;
using RimWorld;
using System.Collections.Generic;
using System.Linq;


namespace VAspirE
{
    [StaticConstructorOnStartup]
    public static class StaticCollectionsClass
    {

        public static Dictionary<RitualBehaviorDef, List<Pawn>> rituals_and_pawns = new Dictionary<RitualBehaviorDef, List<Pawn>>();
        public static Dictionary<Pawn, Pawn> pawns_nuzzled = new Dictionary<Pawn, Pawn>();
        public static Dictionary<Pawn, (RecipeDef,List<Thing>)> pawns_and_completed_recipes = new Dictionary<Pawn, (RecipeDef, List<Thing>)>();
       
        static StaticCollectionsClass()
        {
            List<AspirationDef> toggleableslist = DefDatabase<AspirationDef>.AllDefsListForReading.ToList();

            if (VanillaAspirationsExpanded_Mod.settings.aspirationStates == null) VanillaAspirationsExpanded_Mod.settings.aspirationStates = new Dictionary<string, bool>();

            foreach (AspirationDef toggleabledef in toggleableslist)
            {

                if (!VanillaAspirationsExpanded_Mod.settings.aspirationStates.ContainsKey(toggleabledef.defName) && DefDatabase<AspirationDef>.GetNamedSilentFail(toggleabledef.defName) != null)
                {
                    VanillaAspirationsExpanded_Mod.settings.aspirationStates[toggleabledef.defName] = true;
                }


            }

        }



    }
}
