using RimWorld;
using UnityEngine;
using Verse;

using System.Collections.Generic;
using System.Linq;
using System;



namespace VAspirE
{


    public class VanillaAspirationsExpanded_Settings : ModSettings

    {

        private static Vector2 scrollPosition = Vector2.zero;
        public Dictionary<string, bool> aspirationStates = new Dictionary<string, bool>();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref aspirationStates, "aspirationStates", LookMode.Value, LookMode.Value, ref aspirationKeys, ref boolValues);



        }
        private List<string> aspirationKeys;
        private List<bool> boolValues;
        private string searchKey;



        public void DoWindowContents(Rect inRect)
        {
            var rect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
            Text.Anchor = TextAnchor.MiddleLeft;
            var searchLabel = new Rect(rect.x + 5, rect.y, 60, 24);
            Widgets.Label(searchLabel, "VAspirE.Search".Translate());
            var searchRect = new Rect(searchLabel.xMax + 5, searchLabel.y, 200, 24f);
            searchKey = Widgets.TextField(searchRect, searchKey);
            Text.Anchor = TextAnchor.UpperLeft;


            List<string> keys = aspirationStates.Keys.ToList().OrderBy(
                x => DefDatabase<AspirationDef>.GetNamedSilentFail(x)?.label).Where(x => x.ToLower().Contains(searchKey.ToLower())).ToList();
            Listing_Standard ls = new Listing_Standard();




            Rect rectExt = new Rect(inRect.x, searchRect.yMax + 35, inRect.width, inRect.height - 70);
            Rect rect2 = new Rect(rect.x, rectExt.y, inRect.width - 30f, keys.Count * 24);
            Widgets.BeginScrollView(rectExt, ref scrollPosition, rect2, true);
            //ls.ColumnWidth = rect2.width / 2.2f;
            ls.Begin(rect2);

            for (int num = keys.Count - 1; num >= 0; num--)
            {
                // if (num == keys.Count / 2) { ls.NewColumn(); }
                bool test = aspirationStates[keys[num]];

                if (DefDatabase<AspirationDef>.GetNamedSilentFail(keys[num]) == null)
                {
                    aspirationStates.Remove(keys[num]);
                }
                else
                {

                    var iconRect = new Rect(0, num * 24, 24, 24);
                    var labelRect = new Rect(30, num * 24, inRect.width - 100f, 24);
                  
                    Widgets.DrawTextureFitted(iconRect, DefDatabase<AspirationDef>.GetNamedSilentFail(keys[num]).Icon, 1);
                    Widgets.CheckboxLabeled(labelRect, "VAspirE.Enabled".Translate(DefDatabase<AspirationDef>.GetNamedSilentFail(keys[num]).LabelCap), ref test);

                    aspirationStates[keys[num]] = test;
                }


            }

            ls.End();
            Widgets.EndScrollView();
            base.Write();


        }



    }










}
