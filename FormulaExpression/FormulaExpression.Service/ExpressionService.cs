﻿using FormulaExpression.Infrastructure.Configuration;
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
                command.CommandText = @"SELECT  formula_Log.*, tz_Formula.*
                                        FROM    tz_Formula INNER JOIN
                                                formula_Log ON tz_Formula.KeyID = formula_Log.KeyID
                                        WHERE   (tz_Formula.OrganizationID = @organizationId) AND ((formula_Log.EffectiveDate < { fn NOW() }) AND (formula_Log.ExpirationDate IS NULL) OR
                                                                                                   (formula_Log.EffectiveDate < { fn NOW() }) AND (formula_Log.ExpirationDate > { fn NOW() })
                                                )";

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
        /// 按分厂ID新建公式组
        /// </summary>
        /// <param name="organizationId"></param>
        public static Guid CreateNewFormulaGroup(int organizationId)
        {
            Guid id = Guid.NewGuid();

            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO tz_Formula (KeyID, FactoryID, Name, CreateDate, State) VALUES ('" + id.ToString() + "', " + organizationId + ", '新公式组', '" + DateTime.Now.ToString() + "', 1)";

                connection.Open();
                command.ExecuteNonQuery();
            }

            return id;
        }

        /// <summary>
        /// 更新公式组名称
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="name"></param>
        public static void SaveFormulaGroupName(Guid groupId, string name)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE tz_Formula SET Name = '" + name + "' WHERE KeyID = '" + groupId.ToString() + "'";

                connection.Open();
                command.ExecuteNonQuery();

            }
        }

        /// <summary>
        /// 保存公式
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="data"></param>
        public static void SaveFormulas(Guid groupId, DataTable data)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            // 删除现存公式
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            Delete delete = new Delete("Formula");
            delete.AddCriterions("GroupID", groupId, SqlServerDataAdapter.Infrastruction.CriteriaOperator.Equal);
            factory.Remove(delete);

            // 插入所有公式
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();

                connection.Open();
                foreach (DataRow row in data.Rows)
                {
                    command.CommandText = "INSERT INTO Formula (GroupID, LevelCode, Name, Formula) VALUES ('" + groupId.ToString() + "', '" + row["LevelCode"].ToString() + "','" + row["Name"].ToString() + "','" + row["Formula"].ToString() + "')";
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}