namespace Drugs.Library.Models;

public record SideEffect
{
    public int SideEffectId { get; init; }
    public string Name { get; init; }

    public SideEffect()
    {
    }

    public SideEffect(int sideEffectId, string name)
    {
        SideEffectId = sideEffectId;
        Name = name;
    }
}
