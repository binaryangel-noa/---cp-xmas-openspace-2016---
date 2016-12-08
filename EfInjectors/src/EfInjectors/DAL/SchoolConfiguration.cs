using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace EfInjectors.DAL
{
    public class SchoolConfiguration : DbConfiguration
    {
        public SchoolConfiguration()
        {
            //SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
            AddInterceptor(new TenantCommandInterceptor());
            AddInterceptor(new TenantCommandTreeInterceptor());
        }
    }
}