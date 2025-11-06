using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    // Dependency: StoreContext allows access to the database through EF Core
    private readonly StoreContext context;

    // Constructor: receives StoreContext from ASP.NET Core's built-in Dependency Injection (DI)
    public ProductsController(StoreContext context)
    {
        this.context = context;
    }
    

    /// <summary>
    /// Handles HTTP GET requests to retrieve all products from the database.
    /// </summary>
    /// <returns>
    /// An asynchronous task that returns an <see cref="ActionResult{T}"/> containing a collection of <see cref="Product"/> entities.
    /// </returns>
    /// <remarks>
    /// This endpoint fetches all available products from the database and returns them as a JSON array.
    /// 
    /// Example request:
    /// GET /api/products
    /// 
    /// Example response:
    /// HTTP 200 OK
    /// [
    ///   {
    ///     "id": 1,
    ///     "name": "Laptop",
    ///     "price": 1299.99,
    ///     "description": "High-performance business laptop"
    ///   },
    ///   {
    ///     "id": 2,
    ///     "name": "Mouse",
    ///     "price": 25.50,
    ///     "description": "Wireless optical mouse"
    ///   }
    /// ]
    /// </remarks>
    /// <response code="200">Returns the list of products successfully retrieved from the database.</response>
    /// <response code="500">If there was an internal server error during data retrieval.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProduct()
    {
        // Explanation of return type:
        // Task<>   → because the operation is asynchronous (non-blocking)
        // ActionResult<> → allows returning HTTP responses (e.g. 200 OK, 404 Not Found)
        // IEnumerable<Product> → a collection (list) of Product objects

        // Fetch all products from the database asynchronously
        // ToListAsync() executes a SQL SELECT query via EF Core and returns a list of Product entities
        return await context.Products.ToListAsync();
    }

    [HttpGet("id:int")] // api/products/2
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await context.Products.FindAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        return product;
    }

    /// <summary>
    /// Handles HTTP POST requests to create a new product record in the database.
    /// </summary>
    /// <param name="product">
    /// The <see cref="Product"/> object received from the request body.
    /// This contains the details of the product to be added (e.g., Name, Price, Description).
    /// </param>
    /// <returns>
    /// Returns the newly created <see cref="Product"/> entity with its assigned ID.
    /// </returns>
    /// <remarks>
    /// Example request:
    /// POST /api/products
    /// {
    ///   "name": "Laptop",
    ///   "price": 1299.99,
    ///   "description": "High-performance business laptop"
    /// }
    /// 
    /// Example response:
    /// {
    ///   "id": 101,
    ///   "name": "Laptop",
    ///   "price": 1299.99,
    ///   "description": "High-performance business laptop"
    /// }
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        // Adds the new Product entity to the EF Core change tracker.
        // The record is not yet saved in the database at this point.
        context.Products.Add(product);

        // Saves all pending changes in the context to the database asynchronously.
        // This generates and executes an INSERT SQL statement under the hood.
        await context.SaveChangesAsync();

        // Returns the newly created Product entity as the HTTP response.
        // ASP.NET Core automatically serializes it to JSON (by default).
        return product;
    }




}
