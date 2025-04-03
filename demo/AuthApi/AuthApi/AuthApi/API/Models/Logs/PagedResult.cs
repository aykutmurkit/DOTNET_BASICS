using System.Collections.Generic;

namespace AuthApi.Models.Logs
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        
        public int PageNumber { get; set; }
        
        public int PageSize { get; set; }
        
        public int TotalPages { get; set; }
        
        public long TotalCount { get; set; }
        
        public bool HasPreviousPage => PageNumber > 1;
        
        public bool HasNextPage => PageNumber < TotalPages;
        
        public PagedResult()
        {
            Items = new List<T>();
        }
    }
} 