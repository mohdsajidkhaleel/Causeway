using CargoManagement.Models.Shared;
using Microsoft.EntityFrameworkCore;

namespace CargoManagement.Services.Extensions
{
    public static class IQueryableExtension
    {
        public static PaginatedResponse PaginatedResponse<T>(this IQueryable<T> query,
                                                 int PageNumber, int PageSize
                                                 )
        {
            int SkipTo = (PageNumber * PageSize) - PageSize;

            return new PaginatedResponse { List = query.Skip(SkipTo).Take(PageSize).ToList(), TotalCount = query.Count() };
        }


    }
}
