namespace Deporcheves.Domain.Entities;

public class FixedCost
{
    public int Id {get; private set; }
    public string Name {get; private set;}
    public decimal MonthlyAmount {get; private set; }
    public bool IsActive { get; private set;}
    public DateTime CreatedAt{get; private set;}

    //Contructor para EF Core
    protected FixedCost()
    {
        Name = null!;
    }

    public FixedCost(string name, decimal monthlyAmount)
    {
        if(string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del costo fijo es obligatorio");

        if(monthlyAmount <= 0)
            throw new ArgumentException("El monto mensual debe ser mayor a 0");

        Name = name.Trim();
        MonthlyAmount = monthlyAmount;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }
    public void UpdateAmount(decimal newAmount)
    {
        if(newAmount <= 0)
            throw new ArgumentException("El monto mensual debe ser mayor a 0");

        MonthlyAmount = newAmount;
    }

    public void Desactive() => IsActive = false;
}