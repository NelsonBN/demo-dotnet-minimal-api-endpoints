using System.Collections.Concurrent;
using Demo.Api;

namespace Api;

public interface IProductDAO
{
    void Add(Product product);
    void Delete(int id);
    bool Exists(int id);
    IEnumerable<Product> Get();
    Product? Get(int id);
    void Update(Product product);
}

public class ProductDAO : IProductDAO
{
    private readonly ConcurrentDictionary<int, Product> _db = new();
    private int _lastId = 0;

    public void Add(Product product)
    {
        product.Id = ++_lastId;
        _db.TryAdd(product.Id, product);
    }

    public IEnumerable<Product> Get()
        => _db.Values;

    public Product? Get(int id)
        => _db.TryGetValue(id, out var product)
            ? product
            : null;

    public bool Exists(int id)
        => _db.ContainsKey(id);

    public void Update(Product product)
        => _db.TryUpdate(product.Id, product, product);

    public void Delete(int id)
        => _db.TryRemove(id, out _);
}
