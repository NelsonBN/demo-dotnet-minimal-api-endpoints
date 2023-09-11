using Api;
using Demo.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Tests;

namespace tests;

public class Tests
{
    [Fact]
    public void NoProducts_GetAll_200AndEmptyList()
    {
        // Arrange
        var dao = new FakerProductDAO();

        // Act
        var act = ProductsEndpoints.GetProducts(dao);
        var result = (Ok<IEnumerable<ProductResponse>>)act;


        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public void HasProducts_GetAll_200AndList()
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
        var act = ProductsEndpoints.GetProducts(dao);
        var result = (Ok<IEnumerable<ProductResponse>>)act;


        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().HaveCount(1);
    }




    [Fact]
    public void NonExistentProduct_Get_404()
    {
        // Arrange
        var dao = new FakerProductDAO();


        // Act
        var act = ProductsEndpoints.GetProduct(1, dao);
        var result = (NotFound)act;


        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

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




    [Fact]
    public void NonExistentProduct_Update_404()
    {
        // Arrange
        var dao = new FakerProductDAO();

        var request = new ProductRequest("Test", 1.99);


        // Act
        var act = ProductsEndpoints.UpdateProduct(1, request, dao);
        var result = (NotFound)act;


        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public void ExistentProduct_Update_204()
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

        var request = new ProductRequest("new", 1111.99);


        // Act
        var act = ProductsEndpoints.UpdateProduct(product.Id, request, dao);
        var result = (NoContent)act;


        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }




    [Fact]
    public void NonExistentProduct_Delete_404()
    {
        // Arrange
        var dao = new FakerProductDAO();


        // Act
        var act = ProductsEndpoints.DeleteProduct(1, dao);
        var result = (NotFound)act;


        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public void ExistentProduct_Delete_204()
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
        var act = ProductsEndpoints.DeleteProduct(product.Id, dao);
        var result = (NoContent)act;


        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }
}
