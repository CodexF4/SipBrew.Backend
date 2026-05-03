using System;
using System.Collections.Generic;
using System.Text;

namespace SipBrew.Core.Common
{
    public class DataList<T>
    {
        public int Count { get; set; }
        public List<T> Items { get; set; } = new();
    }
}
