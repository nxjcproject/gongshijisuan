using FormulaExpression.Infrastructure.Configuration;
using SqlServerDataAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormulaExpression.Service
{
    public class ExpressionService
    {
        /// <summary>
        /// 验证表达式合法性
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string[] ValidateExpression(string expression)
        {
            List<string> result = new List<string>();
            foreach (var exception in ExpressionHelper.Utility.ValidateExpression(expression))
            {
                result.Add(exception.Message);
            }

            return result.ToArray();
        }

        /// <summary>
        /// 按组织机构ID获取所有公式组
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public static DataTable GetFormulaGroupsByOrganizationId(string organizationId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            Query query = new Query("tz_Formula");
            query.AddCriterion("OrganizationID", organizationId, SqlServerDataAdapter.Infrastruction.CriteriaOperator.Equal);
            query.AddCriterion("ENABLE", true, SqlServerDataAdapter.Infrastruction.CriteriaOperator.Equal);

            return factory.Query(query);
        }

        /// <summary>
        /// 获取生效中的公式组
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public static DataTable GetFormulaGroupsEffectived(string organizationId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            DataSet ds = new DataSet();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT  [formula_Log].*, [tz_Formula].*
                                        FROM    [tz_Formula] INNER JOIN
                                                [formula_Log] ON [tz_Formula].[KeyID] = [formula_Log].[KeyID]
                                        WHERE   [tz_Formula].[OrganizationID] = @organizationId AND [tz_Formula].[ENABLE] = 1";

                command.Parameters.Add(new SqlParameter("organizationId", organizationId));
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(ds);
            }

            return ds.Tables[0];
        }

        /// <summary>
        /// 获取即将生效的公式组
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public static DataTable GetFormulaGroupsPendingEffectived(string organizationId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            DataSet ds = new DataSet();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT  formula_Log.*, tz_Formula.*
                                        FROM    tz_Formula INNER JOIN
                                                formula_Log ON tz_Formula.KeyID = formula_Log.KeyID
                                        WHERE   (tz_Formula.OrganizationID = @organizationId) AND (formula_Log.EffectiveDate > { fn NOW() })";

                command.Parameters.Add(new SqlParameter("organizationId", organizationId));
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(ds);
            }

            return ds.Tables[0];
        }

        /// <summary>
        /// 获取即将过期的公式组
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public static DataTable GetFormulaGroupsPendingExpiration(string organizationId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            DataSet ds = new DataSet();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT  formula_Log.*, tz_Formula.*
                                        FROM    tz_Formula INNER JOIN
                                                formula_Log ON tz_Formula.KeyID = formula_Log.KeyID
                                        WHERE   (tz_Formula.OrganizationID = @organizationId) AND (formula_Log.ExpirationDate > { fn NOW() })";

                command.Parameters.Add(new SqlParameter("organizationId", organizationId));
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(ds);
            }

            return ds.Tables[0];
        }

        /// <summary>
        /// 按公式组KeyID获取熟料实物煤耗报警设置
        /// </summary>
        /// <param name="keyId"></param>
        /// <returns></returns>
        public static DataTable GetCoalConsumptionAlarm(Guid keyId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            Query query = new Query("formula_ConsumptionAlarmSetting");
            query.AddCriterion("KeyID", keyId, SqlServerDataAdapter.Infrastruction.CriteriaOperator.Equal);

            return factory.Query(query);
        }

        /// <summary>
        /// 按公式组KeyID获取所有公式
        /// </summary>
        /// <param name="keyId"></param>
        /// <returns></returns>
        public static DataTable GetFormulasByGroupId(Guid keyId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            Query query = new Query("formula_FormulaDetail");
            query.AddCriterion("KeyID", keyId, SqlServerDataAdapter.Infrastruction.CriteriaOperator.Equal);
            query.OrderByClauses.Add(new SqlServerDataAdapter.Infrastruction.OrderByClause("LevelCode", false));

            return factory.Query(query);
        }

        /// <summary>
        /// 按公式组ID获取公式组信息
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static DataTable GetFormulaGroupInfoByGroupId(Guid groupId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            Query query = new Query("tz_Formula");
            query.AddCriterion("KeyID", groupId, SqlServerDataAdapter.Infrastruction.CriteriaOperator.Equal);

            return factory.Query(query);
        }

        /// <summary>
        /// 按组织机构ID新建公式组
        /// </summary>
        /// <param name="organizationId"></param>
        public static Guid CreateNewFormulaGroup(string organizationId)
        {
            Guid id = Guid.NewGuid();

            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO tz_Formula (KeyID, OrganizationID, Name, CreatedDate, State) VALUES ('" + id.ToString() + "', '" + organizationId + "', '新公式组', '" + DateTime.Now.ToString() + "', 1)";

                connection.Open();
                command.ExecuteNonQuery();
            }

            return id;
        }

        /// <summary>
        /// 更新公式组名称
        /// </summary>
        /// <param name="keyId"></param>
        /// <param name="name"></param>
        public static void SaveFormulaGroupName(Guid keyId, string name)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE tz_Formula SET Name = '" + name + "' WHERE KeyID = '" + keyId.ToString() + "'";

                connection.Open();
                command.ExecuteNonQuery();

            }
        }

        /// <summary>
        /// 更新报警周期
        /// </summary>
        /// <param name="keyId"></param>
        /// <param name="minutes"></param>
        public static void SaveAlarmPeriod(Guid keyId, int minutes)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE tz_Formula SET AlarmPeriod = " + minutes + " WHERE KeyID = '" + keyId.ToString() + "'";

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 保存公式
        /// </summary>
        /// <param name="keyId"></param>
        /// <param name="data"></param>
        public static void SaveFormulas(Guid keyId, DataTable data)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            // 删除现存公式
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            Delete delete = new Delete("formula_FormulaDetail");
            delete.AddCriterions("KeyID", keyId, SqlServerDataAdapter.Infrastruction.CriteriaOperator.Equal);
            factory.Remove(delete);

            DataColumn variableIdCol = data.Columns["VariableId"];
            variableIdCol.SetOrdinal(0);

            DataColumn keyIdCol = new DataColumn("KeyID", typeof(Guid));
            data.Columns.Add(keyIdCol);
            keyIdCol.SetOrdinal(1);

            DataColumn idCol = new DataColumn("ID", typeof(Guid));
            data.Columns.Add(idCol);
            idCol.SetOrdinal(0);

            foreach (DataRow row in data.Rows)
            {
                row[keyIdCol] = keyId;
                row[idCol] = Guid.NewGuid().ToString();
            }
            // 插入所有公式
            factory.Save("formula_FormulaDetail", data);
        }

        /// <summary>
        /// 保存煤耗报警设置
        /// </summary>
        /// <param name="keyId"></param>
        /// <param name="alarmValue"></param>
        /// <param name="relativeParas"></param>
        public static void SaveCoalConsumptionAlarm(Guid keyId, int alarmValue, string relativeParas)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            // 删除现存公式
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            Delete delete = new Delete("formula_ConsumptionAlarmSetting");
            delete.AddCriterions("KeyID", keyId, SqlServerDataAdapter.Infrastruction.CriteriaOperator.Equal);
            factory.Remove(delete);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO formula_ConsumptionAlarmSetting (KeyID, CoalAlarmValue, RelativeParameters) VALUES ('" + keyId.ToString() + "', " + alarmValue + ", '" + relativeParas + "')";

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 根据公式组ID禁用公式组
        /// </summary>
        /// <param name="groupId"></param>
        public static void DisableFormulasByGroupId(Guid groupId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE tz_Formula SET ENABLE = 0 WHERE KeyID = '" + groupId.ToString() + "'";

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}