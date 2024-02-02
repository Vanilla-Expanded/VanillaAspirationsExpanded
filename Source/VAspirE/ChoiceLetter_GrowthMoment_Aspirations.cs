using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace VAspirE;

public class ChoiceLetter_GrowthMoment_Aspirations : ChoiceLetter_GrowthMoment
{
    public int aspirationChoiceCount;
    public List<AspirationDef> aspirationChoices;
    public int aspirationGainsCount;
    public List<AspirationDef> chosenAspirations;

    public void ConfigureChoiceLetterAspirations(int aspirationChoiceCount, int aspirationGainsCount)
    {
        this.aspirationChoiceCount = aspirationChoiceCount;
        this.aspirationGainsCount = aspirationGainsCount;

        if (passionChoiceCount == 0 && traitChoiceCount == 0 && this.aspirationChoiceCount > 0)
            mouseoverText += "\n\n" + "BirthdayChooseHowPawnWillGrow".Translate(pawn);
    }

    private void TrySetAspirationChoices()
    {
        if (choiceMade || pawn.DestroyedOrNull()) return;

        if (aspirationChoiceCount > 0 && aspirationChoices == null)
            aspirationChoices = DefDatabase<AspirationDef>.AllDefs.Where(aspir => aspir.Worker.ValidOn(pawn))
               .InRandomOrder()
               .Take(aspirationChoiceCount)
               .ToList();
    }

    public void MakeAspirationChoices(List<AspirationDef> aspirations)
    {
        if (ArchiveView) return;

        chosenAspirations = aspirations;

        if (!aspirations.NullOrEmpty()) pawn.needs.Fulfillment().SetAspirations(aspirations);
    }

    public override void OpenLetter()
    {
        TrySetChoices();
        TrySetAspirationChoices();
        var window = new Dialog_GrowthMomentChoices_Aspirations(text, this);
        Find.WindowStack.Add(window);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref aspirationChoiceCount, nameof(aspirationChoiceCount));
        Scribe_Values.Look(ref aspirationGainsCount, nameof(aspirationGainsCount));
        Scribe_Collections.Look(ref aspirationChoices, nameof(aspirationChoices), LookMode.Def);
        Scribe_Collections.Look(ref chosenAspirations, nameof(chosenAspirations), LookMode.Def);
        if (Scribe.mode == LoadSaveMode.PostLoadInit) aspirationChoices?.RemoveAll(a => a == null);
        if (Scribe.mode == LoadSaveMode.PostLoadInit) chosenAspirations?.RemoveAll(a => a == null);
    }
}
