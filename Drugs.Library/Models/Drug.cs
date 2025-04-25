namespace Drugs.Library.Models;

public class Drug
{
    public int DrugId { get; init; }
    public string Name { get; init; }
    public bool IsFavorite { get; set; }

    public Drug()
    {
    }

    public Drug(int drugId, string name, bool isFavorite)
    {
        DrugId = drugId;
        Name = name;
        IsFavorite = isFavorite;
    }
}
