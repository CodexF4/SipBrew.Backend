using System;
using System.Collections.Generic;
using System.Text;

namespace SipBrew.Core.Common
{
    public class PageConfig
    {
        public int? Size { get; set; }
        public int? Index { get; set; }
        public string SortBy { get; set; }
        public bool IsAscending { get; set; } = false;
    }
}
