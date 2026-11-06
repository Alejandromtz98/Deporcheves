namespace Deporcheves.Domain.Entities;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal BaseWeeklySalary { get; set; }
    public bool IsActive { get; set; } = true;
}