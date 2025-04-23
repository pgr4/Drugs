namespace Drugs.Library.Models;

public record DrugSideEffectLink
{
    public int DrugSideEffectLinkId { get; init; }
    public int DrugId { get; init; }
    public int SideEffectId { get; init; }

    public DrugSideEffectLink()
    {
    }

    public DrugSideEffectLink(int drugSideEffectLinkId, int drugId, int sideEffectId)
    {
        DrugSideEffectLinkId = drugSideEffectLinkId;
        DrugId = drugId;
        SideEffectId = sideEffectId;
    }
}
