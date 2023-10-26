namespace ConsoleApp1.Entitys;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Category { get; set; } = null!;
    public int StockQuantity { get; set; }
    public decimal Price { get; set; }
}
