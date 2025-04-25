namespace Drugs.Library.Models;

public class DrugFullInformation
{
    private Drug _drug;
    private IEnumerable<SideEffect> _sideEffects;
    private IEnumerable<Category> _categories;

    public int DrugId { get { return _drug.DrugId; } }
    public string DrugName { get { return _drug.Name; } }
    public bool IsFavorite { get { return _drug.IsFavorite; } }
    public IEnumerable<SideEffect> SideEffects { get { return _sideEffects; } }
    public IEnumerable<Category> Categories { get { return _categories; } }

    public DrugFullInformation(Drug drug, IEnumerable<SideEffect> sideEffects, IEnumerable<Category> categories)
    {
        _drug = drug;
        _sideEffects = sideEffects;
        _categories = categories;
    }

    public bool Like(string input)
    {
        return _drug.Name.Contains(input, StringComparison.OrdinalIgnoreCase);
    }
}
