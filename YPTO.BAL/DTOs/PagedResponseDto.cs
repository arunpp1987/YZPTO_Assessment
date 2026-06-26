using System.Collections.Generic;
using System.Threading.Tasks;
using YPTO.BAL.DTOs;

namespace YPTO.BAL.DTOs
{
    public class PagedResponseDto<T>
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public IEnumerable<T> Data { get; set; } = new List<T>();
    }
}
