using System.Collections.Generic;

namespace LogAPI.Entities.Models
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        
        public int PageNumber { get; set; }
        
        public int PageSize { get; set; }
        
        public long TotalCount { get; set; }
        
        public int TotalPages { get; set; }
        
        public bool HasPreviousPage => PageNumber > 1;
        
        public bool HasNextPage => PageNumber < TotalPages;
    }
} 