using FormulaExpression.Infrastructure.Configuration;
using SqlServerDataAdapter;
using SqlServerDataAdapter.Infrastruction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormulaExpression.Service
{
    public class EquipmentService
    {
        public static DataTable GetEquipmentsByOrganizationId(string organizationId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            Query query = new Query("system_EquipmentAccount");
            query.AddCriterion(new Criterion("OrganizationID", organizationId, CriteriaOperator.Equal));

            return factory.Query(query);
        }
    }
}
