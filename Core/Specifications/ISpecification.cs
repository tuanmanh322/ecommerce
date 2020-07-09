using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Core.Specifications
{
  public interface ISpecification<T>
  {
    // The Specification Pattern will fix certain shortfalls
    // of the Generic Repository pattern.
    Expression<Func<T, bool>> Criteria { get; }

    List<Expression<Func<T, object>>> Includes { get; }
  }
}