using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications
{
  public class ProductsWithTypesAndBrandsSpecification : BaseSpecification<Product>
  {
    public ProductsWithTypesAndBrandsSpecification(ProductSpecParams productParams)
        : base(x => (!productParams.BrandId.HasValue || x.ProductBrandId == productParams.BrandId)
                    && (!productParams.TypeId.HasValue || x.ProductTypeId == productParams.TypeId))
    {
      AddInclude(x => x.ProductBrand);
      AddInclude(x => x.ProductType);
      AddOrderBy(x => x.Name);
      // The PageIndex has to minus one to prevent skipping of
      // pages when it's the first page.
      ApplyPaging(productParams.PageSize * (productParams.PageIndex - 1), productParams.PageSize);

      // Check how data wants to be sorted then 
      // add the appropriate spec.
      if (!string.IsNullOrEmpty(productParams.Sort))
      {
        switch (productParams.Sort)
        {
          case "priceAsc":
            AddOrderBy(p => p.Price);
            break;
          case "priceDesc":
            AddOrderByDescending(p => p.Price);
            break;
          default:
            AddOrderBy(n => n.Name);
            break;
        }
      }
    }

    public ProductsWithTypesAndBrandsSpecification(int id) : base(x => x.Id == id)
    {
      AddInclude(x => x.ProductBrand);
      AddInclude(x => x.ProductType);
    }
  }
}