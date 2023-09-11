using Api;
using Demo.Api;

namespace Tests;

internal class FakerProductDAO : IProductDAO
{
    public Dictionary<int, Product> Database = new();


    public IEnumerable<Product> Get() => Database.Values;
    public Product? Get(int id)
    {
        if(Database.TryGetValue(id, out var product))
        {
            return product;
        }
        return null;
    }

    public void Add(Product product) => product.Id = 1;

    public void Delete(int id) { }
    public bool Exists(int id) => Database.ContainsKey(id);
    public void Update(Product product) { }
}
