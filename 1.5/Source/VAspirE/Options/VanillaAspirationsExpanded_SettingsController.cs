using RimWorld;
using UnityEngine;
using Verse;
using System.Linq;
using System.Collections.Generic;



namespace VAspirE
{



    public class VanillaAspirationsExpanded_Mod : Mod
    {


        public static VanillaAspirationsExpanded_Settings settings;

        public VanillaAspirationsExpanded_Mod(ModContentPack content) : base(content)
        {
            settings = GetSettings<VanillaAspirationsExpanded_Settings>();
        }

        public override string SettingsCategory()
        {
            
                return "VAspirE.ModName".Translate();
            


        }



        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);

            List<AspirationDef> toggleableslist = DefDatabase<AspirationDef>.AllDefsListForReading.ToList();

            if (settings.aspirationStates == null) settings.aspirationStates = new Dictionary<string, bool>();

            foreach (AspirationDef toggleabledef in toggleableslist)
            {

                if (!settings.aspirationStates.ContainsKey(toggleabledef.defName) && DefDatabase<AspirationDef>.GetNamedSilentFail(toggleabledef.defName) != null)
                    {
                        settings.aspirationStates[toggleabledef.defName] = true;
                    }
                

            }

            settings.DoWindowContents(inRect);


        }
    }


}
