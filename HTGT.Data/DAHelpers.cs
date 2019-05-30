using System.Configuration;

namespace HTGT.Data
{
    static class DAHelpers
    {
        internal static readonly string DBConnectionString = ConfigurationManager.ConnectionStrings["htgtconnection"].ConnectionString;
    }
}
