using EfInjectors.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EfInjectors.Models
{
    [TenantAware("TenantId")]
    public class TenantEntity
    {
        public int TenantId { get; set; }
    }
}
