using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.RepositoryBase
{
    public abstract class FilterBase
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
    }
}
