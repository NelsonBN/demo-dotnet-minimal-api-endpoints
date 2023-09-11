namespace Demo.Api;

public record ProductResponse(int Id, string Name, double Price)
{
    public static implicit operator ProductResponse(Product product)
        => new(product.Id,
            product.Name,
            product.Price);
}
