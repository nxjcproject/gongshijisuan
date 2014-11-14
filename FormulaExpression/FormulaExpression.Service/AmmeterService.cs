using FormulaExpression.Infrastructure.Configuration;
using SqlServerDataAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormulaExpression.Service
{
    public class AmmeterService
    {
        public static DataTable GetAmmetersByOrganizationId(Guid organizationId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            Query query = new Query("AmmeterContrast");

            return factory.Query(query);
        }
    }
}
