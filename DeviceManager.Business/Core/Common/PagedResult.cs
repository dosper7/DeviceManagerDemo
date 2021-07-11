using System;
using System.Collections.Generic;
using System.Linq;

namespace DeviceManager.Business.Core.Common
{
    public class PagedResult<T>
    {
        /// <summary>
        /// Number of items returned
        /// </summary>
        public IEnumerable<T> Items { get; set; }
        /// <summary>
        /// Total Count of items
        /// </summary>
        public int TotalCount { get; set; }
    }
}
