namespace Drugs.Library.Models;

public class DrugFullInformation
{
    private Drug _drug;
    private IEnumerable<SideEffect> _sideEffects;

    public int DrugId { get { return _drug.DrugId; } }
    public string DrugName { get { return _drug.Name; } }
    public IEnumerable<SideEffect> SideEffects { get { return _sideEffects; } }

    public DrugFullInformation(Drug drug, IEnumerable<SideEffect> sideEffects)
    {
        _drug = drug;
        _sideEffects = sideEffects;
    }
}
