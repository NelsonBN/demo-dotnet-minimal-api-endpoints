using Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IProductDAO, ProductDAO>();

var app = builder.Build();

app.MapProductsEndpoints();

app.Run();
