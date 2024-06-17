using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace VAspirE;

public class Need_Fulfillment : Need
{
    public List<AspirationDef> Aspirations;
    private List<int> completedTicks;

    public Need_Fulfillment(Pawn pawn) : base(pawn) =>
        threshPercents = new()
        {
            0.25f,
            0.5f,
            0.75f
        };

    public override float MaxLevel => 4;

    public override bool ShowOnNeedList => WorldComponent_PawnList.colonists_with_fulfillment_need.Contains(pawn);

    public int NumCompleted => completedTicks.Count(tick => tick != -1);

    public override void NeedInterval()
    {
        CheckCompletion();
    }

    public override void SetInitialLevel()
    {
        base.SetInitialLevel();
        CurLevelPercentage = 0f;
        Aspirations = new();
        completedTicks = new();
        Rand.PushState(pawn.thingIDNumber);
        var num = Rand.Bool ? 4 : 5;
        for (var i = 0; i < num; i++)
            if (DefDatabase<AspirationDef>.AllDefs
               .Where(aspirationDef => aspirationDef.Worker.ValidOn(pawn) && !Aspirations.Contains(aspirationDef))
               .TryRandomElement(out var aspiration))
            {
                Aspirations.Add(aspiration);
                completedTicks.Add(-1);
            }

        Rand.PopState();
        CheckCompletion();
    }

    public void SetAspirations(List<AspirationDef> aspirationDefs)
    {
        CurLevelPercentage = 0f;
        Aspirations = new();
        completedTicks = new();
        Rand.PushState(pawn.thingIDNumber);
        var num = Rand.Bool ? 4 : 5;
        for (var i = 0; i < num; i++)
            if (i < aspirationDefs.Count)
            {
                Aspirations.Add(aspirationDefs[i]);
                completedTicks.Add(-1);
            }
            else if (DefDatabase<AspirationDef>.AllDefs
                    .Where(aspirationDef => aspirationDef.Worker.ValidOn(pawn) && !Aspirations.Contains(aspirationDef))
                    .TryRandomElement(out var aspiration))
            {
                Aspirations.Add(aspiration);
                completedTicks.Add(-1);
            }

        Rand.PopState();
        CheckCompletion();
    }

    public override void DrawOnGUI(Rect rect, int maxThresholdMarkers = 2147483647, float customMargin = -1, bool drawArrows = true, bool doTooltip = true,
        Rect? rectForTooltip = null, bool drawLabel = true)
    {
        var tooltipRect = rectForTooltip ?? rect;
        var margin = customMargin >= 0f ? customMargin : 14f + 15f;
        tooltipRect.height += (rect.width - margin * 2) / 10;
        var aspirsRect = new Rect(rect.x + margin, rect.y + rect.height - 10, rect.width - margin * 2f, (rect.width - margin * 2) / 5);
        for (var i = 0; i < Aspirations.Count; i++)
        {
            var aspirRect = aspirsRect;
            aspirRect.width /= 5;
            aspirRect.x += aspirRect.width * i;

            if (Aspirations[i].Worker.IsCompleted(this.pawn))
            {
                GUI.DrawTexture(aspirRect, Aspirations[i].Icon);
            }
            else { GUI.DrawTexture(aspirRect, Aspirations[i].Icon, ScaleMode.ScaleToFit, true, 1, Color.gray, 0, 0); }
            
            if (Mouse.IsOver(aspirRect))
            {
                Widgets.DrawHighlight(aspirRect);
                doTooltip = false;
                TooltipHandler.TipRegion(aspirRect, Aspirations[i].TooltipFor(pawn, completedTicks[i]));
            }
        }

        base.DrawOnGUI(rect, maxThresholdMarkers, customMargin, drawArrows, doTooltip, tooltipRect, drawLabel);
    }

    public void Complete(AspirationDef def)
    {
        var idx = Aspirations.IndexOf(def);
        if (idx == -1) return;
        if (completedTicks[idx] != -1) return;
        completedTicks[idx] = GenTicks.TicksAbs;
        Messages.Message("VAspireE.AspirationComplete".Translate(pawn.NameFullColored, def.LabelCap), pawn, MessageTypeDefOf.PositiveEvent);
        CurLevel += 1;
        if (CurLevel >= 4)
        {
            var letter = (ChoiceLetter_AspirationsFulfilled)LetterMaker.MakeLetter(AspirationDefOf.VAspirE_Fulfilled);
            letter.ConfigureFulfillmentLetter(pawn);
            Find.LetterStack.ReceiveLetter(letter, "AspirationsFulilled");
        }
    }

    public bool IsComplete(AspirationDef def)
    {
        var idx = Aspirations.IndexOf(def);
        if (idx == -1) return false;
        return completedTicks[idx] != -1;
    }

    public void CheckCompletion()
    {
        foreach (var aspirationDef in Aspirations)
            try
            {
                if (!IsComplete(aspirationDef))
                    if (aspirationDef.Worker.IsCompleted(pawn))
                        Complete(aspirationDef);
            }
            catch (Exception e) { Log.Error($"[VAspirE] Exception while checking completion of {aspirationDef}: {e}"); }
    }

    public void DebugAddAspiration(AspirationDef def)
    {
        Aspirations.Add(def);
        completedTicks.Add(-1);
    }

    public void DebugRemoveAspiration(AspirationDef def)
    {
        var i = Aspirations.IndexOf(def);
        if (i == -1) return;
        Aspirations.RemoveAt(i);
        completedTicks.RemoveAt(i);
    }

  

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref Aspirations, "aspirations", LookMode.Def);
        Scribe_Collections.Look(ref completedTicks, nameof(completedTicks), LookMode.Value);
    }
}
