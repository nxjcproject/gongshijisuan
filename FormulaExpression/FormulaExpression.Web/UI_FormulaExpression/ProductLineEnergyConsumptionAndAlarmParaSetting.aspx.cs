using EasyUIJsonParser;
using FormulaExpression.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FormulaExpression.Web.UI_FormulaExpression
{
    public partial class ProductLineEnergyConsumptionAndAlarmParaSetting : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        [WebMethod]
        public static string ValidateExpression(string expression)
        {
            string[] result = ExpressionService.ValidateExpression(expression);
            if (result.Length == 0)
                return "success";
            else
                return result[0];
        }

        [WebMethod]
        public static string GetFormulaName(string keyId)
        {
            Guid id = new Guid(keyId);

            DataTable dt = ExpressionService.GetFormulaGroupInfoByGroupId(id);
            if (dt.Rows.Count > 0)
                return "{\"name\":\"" + dt.Rows[0]["Name"].ToString() + "\"}";
            else
                return "";
        }

        [WebMethod]
        public static void UpdateFormulaGroupName(string keyId, string name)
        {
            Guid id = new Guid(keyId);

            ExpressionService.SaveFormulaGroupName(id, name);
        }

        /// <summary>
        /// 获取煤耗报警
        /// </summary>
        /// <param name="keyId"></param>
        /// <returns></returns>
        [WebMethod]
        public static string GetCoalConsumptionAlarm(string keyId)
        {
            Guid id = new Guid(keyId);
            DataTable dt = ExpressionService.GetCoalConsumptionAlarm(id);
            string alarmValue = dt.Rows[0]["CoalAlarmValue"].ToString();
            string relativeParas = dt.Rows[0]["RelativeParameters"].ToString();
            return "{\"alarmValue\":\"" + alarmValue + "\",\"relativeParas\":\"" + relativeParas + "\"}";
        }

        /// <summary>
        /// 获取所有公式
        /// </summary>
        /// <param name="keyId"></param>
        /// <returns></returns>
        [WebMethod]
        public static string GetFormulasWithTreeGridFormat(string keyId)
        {
            Guid id = new Guid(keyId);
            DataTable formulas = ExpressionService.GetFormulasByGroupId(id);
            DataColumn parentIdColumn = new DataColumn("ParentID");
            formulas.Columns.Add(parentIdColumn);

            foreach (DataRow row in formulas.Rows)
            {
                string levelcode = row["LevelCode"].ToString().Trim();
                if (levelcode.Length > 3)
                    row["ParentID"] = levelcode.Substring(0, levelcode.Length - 2);
            }

            return TreeGridJsonParser.DataTableToJson(formulas, "LevelCode", "ParentID", "Name", "Formula", "Denominator", "Required", "AlarmType", "EnergyAlarmValue", "PowerAlarmValue", "RelativeParameters", "Remarks");
        }

        /// <summary>
        /// 保存所有公式
        /// </summary>
        /// <param name="keyId"></param>
        /// <param name="json"></param>
        [WebMethod]
        public static void SaveFormulasWithTreeGridFormat(string keyId, string json)
        {
            Guid id = new Guid(keyId);
            DataTable dt = TreeGridJsonParser.JsonToDataTable(json);
            
            ExpressionService.SaveFormulas(id, dt);
        }

        /// <summary>
        /// 保存煤耗报警设置
        /// </summary>
        /// <param name="keyId"></param>
        /// <param name="alarmValue"></param>
        /// <param name="relativeParas"></param>
        [WebMethod]
        public static void SaveCoalConsumptionAlarm(string keyId, string alarmValue, string relativeParas)
        {
            ExpressionService.SaveCoalConsumptionAlarm(new Guid(keyId), int.Parse(alarmValue), relativeParas);
        }

        /// <summary>
        /// 获取电表信息
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        [WebMethod]
        public static string GetAmmeterLabelsWithTreeGridFormat(string organizationId)
        {
            Guid id = new Guid(organizationId);
            DataTable dt = AmmeterService.GetAmmetersByOrganizationId(id);

            return TreeGridJsonParser.DataTableToJson(dt, "ElectricRoom", new string[] { "AmmeterNumber", "AmmeterName" });
        }

    }
}