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

          

            settings.DoWindowContents(inRect);


        }
    }


}
