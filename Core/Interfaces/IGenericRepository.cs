using Core.Entities;

namespace Core.Interfaces;


/// <summary>
/// Represents a generic repository for performing data access operations
/// on entities of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">
/// The type of entity the repository will operate on. 
/// Must inherit from <see cref="BaseEntity"/>.
/// </typeparam>
/// <remarks>
/// This interface can be implemented for any entity in the system
/// to provide consistent CRUD operations. 
/// It supports type safety and code reuse through generics.
/// </remarks>
public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> ListAllAsync();
    void Add(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task<bool> SaveAllAsync();
    bool Exists(int id);

}
