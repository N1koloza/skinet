using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controller responsible for handling CRUD operations for <see cref="Product"/> entities.
/// This class uses dependency injection to access a repository implementing <see cref="IProductRepository"/>.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductRepository repo) : ControllerBase
{
    // =====================
    //        GET ALL
    // =====================

    /// <summary>
    /// Retrieves all products from the database.
    /// </summary>
    /// <returns>
    /// An asynchronous task that returns an <see cref="ActionResult{T}"/> containing 
    /// a read-only list of <see cref="Product"/> entities.
    /// </returns>
    /// <remarks>
    /// This endpoint fetches all products asynchronously and returns them as a JSON array.
    /// 
    /// Example request:
    /// <code>GET /api/products</code>
    ///
    /// Example response:
    /// <code>
    /// HTTP 200 OK
    /// [
    ///   { "id": 1, "name": "Laptop", "price": 1299.99 },
    ///   { "id": 2, "name": "Mouse", "price": 25.50 }
    /// ]
    /// </code>
    /// </remarks>
    /// <response code="200">Returns the list of products successfully retrieved.</response>
    /// <response code="500">If an internal server error occurred during retrieval.</response>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProduct()
    {
        // Asynchronously retrieves all Product entities from the repository.
        // The repository handles EF Core query execution and context management.
        var products = await repo.GetProductsAsync();

        // Return the product list wrapped in a 200 OK HTTP response.
        return Ok(products);
    }

    // =====================
    //       GET BY ID
    // =====================

    /// <summary>
    /// Retrieves a single product by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the product to retrieve.</param>
    /// <returns>
    /// An <see cref="ActionResult{T}"/> containing the requested <see cref="Product"/> 
    /// if found, or a 404 Not Found response if the product does not exist.
    /// </returns>
    /// <remarks>
    /// Example request:
    /// <code>GET /api/products/5</code>
    /// </remarks>
    /// <response code="200">Returns the requested product.</response>
    /// <response code="404">If the product with the specified ID does not exist.</response>
    [HttpGet("{id:int}")] // Route example: /api/products/2
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        // Attempt to retrieve the product by ID from the repository.
        var product = await repo.GetProductByIdAsync(id);

        // If not found, return HTTP 404.
        if (product == null)
        {
            return NotFound();
        }

        // Return the product as the response.
        return product;
    }

    // =====================
    //         CREATE
    // =====================

    /// <summary>
    /// Creates a new product record in the database.
    /// </summary>
    /// <param name="product">
    /// The <see cref="Product"/> object provided in the request body containing product details.
    /// </param>
    /// <returns>
    /// Returns the newly created product with its assigned ID.
    /// </returns>
    /// <remarks>
    /// Example request:
    /// <code>
    /// POST /api/products
    /// {
    ///   "name": "Laptop",
    ///   "price": 1299.99,
    ///   "description": "High-performance business laptop"
    /// }
    /// </code>
    ///
    /// Example response:
    /// <code>
    /// HTTP 201 Created
    /// {
    ///   "id": 101,
    ///   "name": "Laptop",
    ///   "price": 1299.99,
    ///   "description": "High-performance business laptop"
    /// }
    /// </code>
    /// </remarks>
    /// <response code="201">Successfully created a new product.</response>
    /// <response code="400">If the product could not be created.</response>
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        // Track the new product entity within the repository.
        repo.AddProduct(product);

        // Attempt to save changes to the database asynchronously.
        if (await repo.SaveChangesAsync())
        {
            // Return HTTP 201 Created, including the location header for the new resource.
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // Return 400 Bad Request if saving failed.
        return BadRequest("Problem creating the product");
    }

    // =====================
    //         UPDATE
    // =====================

    /// <summary>
    /// Updates an existing product in the database.
    /// </summary>
    /// <param name="id">The ID of the product to update.</param>
    /// <param name="product">The updated <see cref="Product"/> data from the request body.</param>
    /// <returns>
    /// A 204 No Content response on success, or 400/404 on validation errors.
    /// </returns>
    /// <remarks>
    /// Example request:
    /// <code>
    /// PUT /api/products/5
    /// {
    ///   "id": 5,
    ///   "name": "Updated Laptop",
    ///   "price": 1399.99,
    ///   "description": "Updated description",
    ///   "brand": "Updated brand",
    ///   "type": "Updated type",
    ///   "quantityInStock": 50
    /// }
    /// </code>
    /// </remarks>
    /// <response code="204">Successfully updated the product.</response>
    /// <response code="400">If the product IDs mismatch or the product doesn't exist.</response>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<Product>> UpdateProduct(int id, Product product)
    {
        // Validation check: ensure ID in URL matches the product's ID.
        if (product.Id != id || !ProductExists(id))
        {
            return BadRequest("Cannot update this product");
        }

        // Mark the entity as modified for EF Core tracking.
        repo.UpdateProduct(product);

        // Save updates to the database.
        if (await repo.SaveChangesAsync())
        {
            return NoContent();
        }

        return BadRequest("Problem updating the product");
    }

    // =====================
    //         DELETE
    // =====================

    /// <summary>
    /// Deletes a product record from the database.
    /// </summary>
    /// <param name="id">The ID of the product to delete.</param>
    /// <returns>
    /// A 204 No Content response on success, or 404 if not found.
    /// </returns>
    /// <remarks>
    /// Example request:
    /// <code>DELETE /api/products/5</code>
    /// </remarks>
    /// <response code="204">Product deleted successfully.</response>
    /// <response code="404">If the product with the specified ID does not exist.</response>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        // Attempt to retrieve the product.
        var product = await repo.GetProductByIdAsync(id);

        // Return 404 if not found.
        if (product == null) return NotFound();

        // Remove the product entity from the context.
        repo.DeleteProduct(product);

        // Commit deletion to the database.
        if (await repo.SaveChangesAsync())
        {
            return NoContent();
        }

        return BadRequest("Problem deleting the product");
    }

    // =====================
    //      UTILITIES
    // =====================

    /// <summary>
    /// Checks if a product exists in the repository by ID.
    /// </summary>
    /// <param name="id">The product ID to check.</param>
    /// <returns>True if the product exists, otherwise false.</returns>
    private bool ProductExists(int id)
    {
        return repo.ProductExists(id);
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    {
        return Ok(await repo.GetBrandsAsync());
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        return Ok(await repo.GetTypesAsync());
    }
    
}
