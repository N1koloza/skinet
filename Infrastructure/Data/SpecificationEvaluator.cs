using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data;

public class SpecificationEvaluator <T> where T : BaseEntity
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> spec)
    {
        var query = inputQuery;

        // Apply criteria if it exists
        if (spec.Criteria != null)
        {
            query = query.Where(spec.Criteria); // x = x.Brand == "Apple"
        }

        return query;
    }
}
