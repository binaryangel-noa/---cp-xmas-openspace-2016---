using System;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace OSD.FactoryFramework
{
    /// <summary>
    /// The factory aware attribute
    /// </summary>
    /// <seealso cref="System.Attribute" />
    public class EdmHelper : Attribute
    {
        /// <summary>
        /// The factory column name
        /// </summary>
        private const string FactoryColumnName = "TenantId";

        /// <summary>
        /// Gets the name of the factory column.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>name of tentant column if exists</returns>
        public static string GetFactoryColumnName(EdmType type)
        {
            return ((System.Data.Entity.Core.Metadata.Edm.StructuralType)type).Members.Any(x => x.Name == "TenantId") ? "TenantId" : null;
        }
    }
}