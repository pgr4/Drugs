namespace Drugs.Library.Models;

public class Drug
{
    public int DrugId { get; set; }
    public string Name { get; set; }

    public Drug()
    {
    }

    public Drug(int drugId, string name)
    {
        DrugId = drugId;
        Name = name;
    }
}
