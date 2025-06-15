using RimWorld;
using Verse;

namespace VAspirE;

public class ChoiceLetter_AspirationsFulfilled : ChoiceLetter_GrowthMoment
{
    public void ConfigureFulfillmentLetter(Pawn pawn)
    {
        ConfigureGrowthLetter(pawn, 6, 6, 3, null, pawn.Name);
        growthTier = -1;
        Label = "BirthdayGrowthMoment".Translate(pawn, pawn.NameShortColored.Named("PAWNNAME"));
        text = "VAspirE.FullfilledGrowthMoment".Translate(pawn.NameShortColored).CapitalizeFirst();
        text += "\n\n";
        text += "VAspirE.FullfilledGrowthMoment.Desc".Translate(pawn);
        mouseoverText = text;
    }

    public override void OpenLetter()
    {
        TrySetChoices();
        var window = new Dialog_AspirationsFulfilled(text, this);
        Find.WindowStack.Add(window);
    }
}
