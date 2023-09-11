using Demo.Api;

namespace Api;

public static class ProductsEndpoints
{
    public static void MapProductsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/products");

        group.MapGet("", GetProducts)
            .WithName(nameof(GetProducts))
            .Produces<Product[]>()
            .Produces(StatusCodes.Status200OK);

        group.MapGet("{id}", GetProduct)
            .WithName(nameof(GetProduct))
            .Produces<ProductResponse>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("", CreateProduct)
            .WithName(nameof(CreateProduct))
            .Produces<ProductResponse>()
            .Produces(StatusCodes.Status201Created);

        group.MapPut("{id}", UpdateProduct)
            .WithName(nameof(UpdateProduct))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("{id}", DeleteProduct)
            .WithName(nameof(DeleteProduct))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    public static IResult GetProducts(IProductDAO dao)
    {
        var products = dao
            .Get()
            .Select(product => (ProductResponse)product);

        return Results.Ok(products);
    }

    public static IResult GetProduct(int id, IProductDAO dao)
    {
        var product = dao.Get(id);
        if(product is null)
        {
            return Results.NotFound();
        }

        return Results.Ok((ProductResponse)product);
    }

    public static IResult CreateProduct(ProductRequest request, IProductDAO dao)
    {
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price
        };

        dao.Add(product);

        return Results.CreatedAtRoute(
            nameof(GetProduct),
            new { id = product.Id.ToString() },
            (ProductResponse)product);
    }

    public static IResult UpdateProduct(int id, ProductRequest request, IProductDAO dao)
    {
        var product = dao.Get(id);
        if(product is null)
        {
            return Results.NotFound();
        }

        product.Name = request.Name;
        product.Price = request.Price;

        dao.Update(product);

        return Results.NoContent();
    }

    public static IResult DeleteProduct(int id, IProductDAO dao)
    {
        if(!dao.Exists(id))
        {
            return Results.NotFound();
        }

        dao.Delete(id);

        return Results.NoContent();
    }
}
