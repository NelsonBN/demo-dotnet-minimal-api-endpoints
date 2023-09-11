# Demo how we can organize the endpoints with minimal APIs

This is a demo showing how we can organize the endpoints with minimal APIs.

With this approach, the code stays clean, easy to test and we can isolate the responsibilities.

## Endpoints declaration

```csharp
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

    ...
    ...
}
```

## Endpoints behavior

```csharp
public static class ProductsEndpoints
{
    ...
    ...

    public static IResult GetProducts(IProductDAO dao)
    {
        var products = dao.Get();

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
```

## Endpoints tests

```csharp
public class Tests
{
    ...
    ...

    [Fact]
    public void ExistentProduct_Get_200()
    {
        // Arrange
        var dao = new FakerProductDAO();

        var product = new Product
        {
            Id = 11,
            Name = "Test",
            Price = 1.99
        };
        dao.Database[product.Id] = product;


        // Act
        var act = ProductsEndpoints.GetProduct(product.Id, dao);
        var result = (Ok<ProductResponse>)act;


        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);

        result.Value!.Id.Should().Be(product.Id);
        result.Value.Name.Should().Be(product.Name);
        result.Value.Price.Should().Be(product.Price);
    }

    [Fact]
    public void NewProduct_Add_201AndId1()
    {
        // Arrange
        var dao = new FakerProductDAO();

        var request = new ProductRequest("Test", 1.99);


        // Act
        var act = ProductsEndpoints.CreateProduct(request, dao);
        var result = (CreatedAtRoute<ProductResponse>)act;


        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status201Created);

        result.Value!.Id.Should().Be(1);
        result.Value.Name.Should().Be(request.Name);
        result.Value.Price.Should().Be(request.Price);
    }

    ...
    ...
}
```