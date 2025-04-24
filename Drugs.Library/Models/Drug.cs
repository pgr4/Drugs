namespace Drugs.Library.Models;

public class Drug
{
    public int DrugId { get; init; }
    public string Name { get; init; }

    public Drug()
    {
    }

    public Drug(int drugId, string name)
    {
        DrugId = drugId;
        Name = name;
    }
}
