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

    [HttpGet("{id:int}")]// api/products/2
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


    /// <summary>
    /// Handles HTTP PUT requests to update an existing product in the database.
    /// </summary>
    /// <param name="id">
    /// The ID of the product to update. This is usually provided in the URL path.
    /// </param>
    /// <param name="product">
    /// The <see cref="Product"/> object containing updated product information from the request body.
    /// </param>
    /// <returns>
    /// Returns an <see cref="ActionResult{Product}"/> indicating the result of the operation:
    /// - 204 No Content: Successfully updated the product
    /// - 400 Bad Request: If the product IDs do not match or the product does not exist
    /// </returns>
    /// <remarks>
    /// Example request:
    /// PUT /api/products/5
    /// {
    ///   "id": 5,
    ///   "name": "Updated Laptop",
    ///   "price": 1399.99,
    ///   "description": "Updated description",
    ///   "brand": "Updated brand",
    ///   "type": "Updated type",
    ///   "quantityInStock": 50,
    ///   "pictureUrl": "https://example.com/updated-image.jpg"
    /// }
    /// </remarks>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<Product>> UpdateProduct(int id, Product product)
    {
        // Validate the request:
        // Check if the ID in the URL matches the product's ID in the body
        // Also check if the product actually exists in the database
        if (product.Id != id || !ProductExists(id))
        {
            // Return 400 Bad Request if IDs do not match or product does not exist
            return BadRequest("Cannot update this product");
        }

        // Mark the entity as modified:
        // This tells Entity Framework Core that the product entity has been changed
        // EF Core will generate an UPDATE SQL command for this entity on SaveChangesAsync()
        context.Entry(product).State = EntityState.Modified;

        // Save changes asynchronously:
        // This executes the UPDATE command in the database
        await context.SaveChangesAsync();

        // Return 204 No Content:
        // Standard REST practice: indicate the update succeeded without returning the updated object
        return NoContent();
    }

    private bool ProductExists(int id)
    {
        return context.Products.Any(x => x.Id == id);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await context.Products.FindAsync(id);

        if (product == null) return NotFound();

        context.Products.Remove(product);

        await context.SaveChangesAsync();

        return NoContent();

    }


}
