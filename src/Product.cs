namespace Demo.Api;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; } = default!;
    public required double Price { get; set; }
}
