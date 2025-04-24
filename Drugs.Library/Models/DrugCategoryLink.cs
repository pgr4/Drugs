namespace Drugs.Library.Models;

public record DrugCategoryLink
{
    public int DrugCategoryLinkId { get; init; }
    public int DrugId { get; init; }
    public int CategoryId { get; init; }

    public DrugCategoryLink()
    {
    }

    public DrugCategoryLink(int drugCategoryLinkId, int drugId, int categoryId)
    {
        DrugCategoryLinkId = drugCategoryLinkId;
        DrugId = drugId;
        CategoryId = categoryId;
    }
}
