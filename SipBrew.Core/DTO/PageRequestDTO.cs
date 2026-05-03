using System;
using System.Collections.Generic;
using System.Text;

namespace SipBrew.Core.DTO
{
    public class PageRequestDTO
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string sortBy { get; set; } = "ID";
        public bool isAscending { get; set; } = true;
    }

    public class PageResponseDTO<T>
    {
        public List<T> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
