using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace VAspirE;

public class Dialog_GrowthMomentChoices_Aspirations : Dialog_GrowthMomentChoices
{
    private readonly List<AspirationDef> chosenAspirations = new();

    private new readonly ChoiceLetter_GrowthMoment_Aspirations letter;

    public Dialog_GrowthMomentChoices_Aspirations(TaggedString text, ChoiceLetter_GrowthMoment_Aspirations letter) : base(text, letter)
    {
        this.letter = letter;
        if (AspirationSelectionsMade())
        {
            closeOnAccept = true;
            closeOnCancel = true;
        }
        else
        {
            closeOnAccept = false;
            closeOnCancel = false;
        }
    }

    private bool AspirationSelectionsMade() =>
        letter.ArchiveView
     || ((letter.passionChoices.NullOrEmpty() || !chosenPassions.NullOrEmpty() || letter.passionGainsCount <= 0)
      && (letter.traitChoices.NullOrEmpty() || chosenTrait != null)
      && (letter.aspirationChoices.NullOrEmpty() || !chosenAspirations.NullOrEmpty() || letter.aspirationGainsCount <= 0));

    private AcceptanceReport CanCloseOverride()
    {
        if (letter.ArchiveView) return true;

        if (!letter.passionChoices.NullOrEmpty() && chosenPassions.Count != letter.passionGainsCount)
            return letter.passionGainsCount == 1 ? "SelectPassionSingular".Translate() : "SelectPassionsPlural".Translate(letter.passionGainsCount);

        if (!letter.traitChoices.NullOrEmpty() && chosenTrait == null)
            return "SelectATrait".Translate();

        if (!letter.aspirationChoices.NullOrEmpty() && chosenAspirations.Count != letter.aspirationGainsCount)
            return letter.aspirationGainsCount == 1
                ? "VAspirE.SelectAspirationSingular".Translate()
                : "VAspirE.SelectAspirationsPlural".Translate(letter.aspirationGainsCount);

        if (!AspirationSelectionsMade())
            return "BirthdayMakeChoices".Translate();

        return AcceptanceReport.WasAccepted;
    }

    private void DrawAspirationChoices(float width, ref float curY)
    {
        if (!letter.ArchiveView && !letter.aspirationChoices.NullOrEmpty() && letter.aspirationGainsCount > 0)
        {
            Widgets.Label(0, ref curY, width,
                (letter.aspirationGainsCount == 1
                    ? "VAspirE.BirthdaySelectAspirationSingular"
                    : "VAspirE.BirthdaySelectAspirationsPlural").Translate(letter.pawn.NameShortColored));

            var listing = new Listing_Standard();
            var rect = new Rect(0f, curY, 230f, 99999f);
            listing.Begin(rect);
            foreach (var aspirationDef in letter.aspirationChoices)
            {
                GUI.DrawTexture(new(rect.xMax - 55f, listing.CurHeight, 24f, 24f), aspirationDef.Icon);

                if (letter.aspirationGainsCount > 1)
                {
                    var active = chosenAspirations.Contains(aspirationDef);
                    var oldActive = active;
                    listing.CheckboxLabeled(aspirationDef.LabelCap, ref active, 30f);
                    if (active != oldActive)
                    {
                        if (active)
                            chosenAspirations.Add(aspirationDef);
                        else
                            chosenAspirations.Remove(aspirationDef);
                    }
                }
                else if (listing.RadioButton(aspirationDef.LabelCap, chosenAspirations.Contains(aspirationDef), 30f))
                {
                    chosenAspirations.Clear();
                    chosenAspirations.Add(aspirationDef);
                }
            }

            listing.End();
            curY += listing.CurHeight;

            Widgets.Label(0, ref curY, width, "VAspirE.AspirationDesc".Translate(letter.pawn.NameShortColored).Colorize(ColoredText.SubtleGrayColor));
            curY += 14;
        }

        if (letter.ArchiveView && !letter.chosenAspirations.NullOrEmpty())
        {
            Widgets.Label(0f, ref curY, width, letter.chosenAspirations.Count == 1
                ? "VAspirE.AspirationSelectionArchiveSingular".Translate(letter.chosenAspirations[0].LabelCap)
                : "VAspirE.AspirationSelectionArchivePlural".Translate(letter.chosenAspirations.Select(aspir => aspir.LabelCap.Resolve()).ToCommaList(true)));
            curY += 14f;
        }
    }

    public override void DoWindowContents(Rect inRect)
    {
        var showInfoTabs = letter.ShowInfoTabs;
        const float width = 446f;
        var outRect = showInfoTabs ? inRect.LeftPartPixels(width) : inRect;
        outRect.yMax -= 4f + CloseButSize.y;
        Text.Font = GameFont.Small;
        var viewRect = new Rect(outRect.x, outRect.y, outRect.width - 16f, scrollHeight);
        Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
        var curY = 0f;
        Widgets.Label(0f, ref curY, viewRect.width, text.Resolve());
        curY += 14f;
        DrawPassionChoices(viewRect.width, ref curY);
        DrawTraitChoices(viewRect.width, ref curY);
        DrawAspirationChoices(viewRect.width, ref curY);
        DrawBottomText(viewRect.width, ref curY);
        if (Event.current.type == EventType.Layout) scrollHeight = Mathf.Max(curY, outRect.height);
        Widgets.EndScrollView();
        var laterRect = new Rect(0f, outRect.yMax + 4f, inRect.width, CloseButSize.y);
        var canClose = CanCloseOverride();
        var okRect = new Rect(laterRect.xMax - CloseButSize.x, laterRect.y, CloseButSize.x, CloseButSize.y);
        if (letter.ArchiveView)
            okRect.x = laterRect.center.x - CloseButSize.x / 2f;
        else
        {
            if (Widgets.ButtonText(new(laterRect.x, laterRect.y, CloseButSize.x, CloseButSize.y), "Later".Translate()))
            {
                if (letter.ShouldAutomaticallyOpenLetter)
                    Messages.Message("MessageCannotPostponeGrowthMoment".Translate(letter.pawn.Named("PAWN")), null, MessageTypeDefOf.RejectInput, false);
                else
                    Close();
            }

            if (!canClose.Accepted)
            {
                var anchor = Text.Anchor;
                var font = Text.Font;
                Text.Font = GameFont.Tiny;
                Text.Anchor = TextAnchor.MiddleRight;
                var rect3 = laterRect;
                rect3.xMax = okRect.xMin - 4f;
                Widgets.Label(rect3, canClose.Reason.Colorize(ColoredText.WarningColor));
                Text.Font = font;
                Text.Anchor = anchor;
            }
        }

        if (Widgets.ButtonText(okRect, "OK".Translate()))
        {
            if (canClose.Accepted)
            {
                letter.MakeAspirationChoices(chosenAspirations);
                letter.MakeChoices(chosenPassions, chosenTrait);
                Close();
                Find.LetterStack.RemoveLetter(letter);
            }
            else
                Messages.Message(canClose.Reason, null, MessageTypeDefOf.RejectInput, false);
        }

        if (showInfoTabs)
        {
            var rect4 = inRect.RightPartPixels(1000f - outRect.width - 34f);
            rect4.xMin += 17f;
            rect4.yMax -= 4f + CloseButSize.y;
            tmpTabs.Clear();
            tmpTabs.Add(new("TabCharacter".Translate(), delegate { showBio = true; }, showBio));
            tmpTabs.Add(new("TabHealth".Translate(), delegate { showBio = false; }, !showBio));
            rect4.yMin += 32f;
            var rect5 = new Rect(rect4.x + (showBio ? 17f : 0f), rect4.y, rect4.width, rect4.height);
            Widgets.DrawMenuSection(rect4);
            if (showBio)
                CharacterCardUtility.DrawCharacterCard(rect5, letter.pawn, null, default, false);
            else
                HealthCardUtility.DrawHediffListing(rect5, letter.pawn, false, 17f);
            TabDrawer.DrawTabs(rect4, tmpTabs);
            tmpTabs.Clear();
        }
    }
}
