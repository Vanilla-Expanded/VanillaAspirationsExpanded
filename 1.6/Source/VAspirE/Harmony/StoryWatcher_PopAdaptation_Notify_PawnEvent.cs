using HarmonyLib;
using RimWorld;
using Verse;


namespace VAspirE
{

    [HarmonyPatch(typeof(StoryWatcher_PopAdaptation))]
    [HarmonyPatch("Notify_PawnEvent")]
    public static class VAspirE_StoryWatcher_PopAdaptation_Notify_PawnEvent_Patch
    {


        [HarmonyPostfix]
        public static void RefreshNeedList(Pawn p, PopAdaptationEvent ev)
        {
            if (ev == PopAdaptationEvent.GainedColonist)
            {
                WorldComponent_PawnList.RefreshNeedList(p);
            }


        }


    }













}

