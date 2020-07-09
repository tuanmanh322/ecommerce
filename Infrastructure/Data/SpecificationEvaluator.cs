using System.Linq;
using Core.Entities;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
  // This class should be constrain only with BaseEntity classes.
  public class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
  {
    public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery,
                                                  ISpecification<TEntity> spec)
    {
      var query = inputQuery;

      if (spec.Criteria != null)
      {
        query = query.Where(spec.Criteria);
      }

      // Take all Include statements and aggregates them so that ToList, First, 
      // or other ending LINQ clause can be called upon them.
      // E.g. _context.Products.Include(p => p.ProductBrand)Include(p => p.ProductType).....
      query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

      return query;
    }
  }
}