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
    public partial class FormulaGroups : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static string GetFormulaGroupsWithDataGridFormat(string organizationId)
        {
            DataTable formulaGroups = ExpressionService.GetFormulaGroupsByOrganizationId(organizationId);

            return DataGridJsonParser.DataTableToJson(formulaGroups, "KeyID", "Name", "CreatedDate", "State");
        }

        [WebMethod]
        public static string GetFormulaGroupsEffectivedWithDataGridFormat(string organizationId)
        {
            DataTable formulaGroups = ExpressionService.GetFormulaGroupsEffectived(organizationId);

            return DataGridJsonParser.DataTableToJson(formulaGroups, "KeyID", "Name", "CreatedDate", "State", "EffectiveDate", "ExpirationDate");
        }

        [WebMethod]
        public static string GetFormulaGroupsPendingEffectivedWithDataGridFormat(string organizationId)
        {
            DataTable formulaGroups = ExpressionService.GetFormulaGroupsPendingEffectived(organizationId);

            return DataGridJsonParser.DataTableToJson(formulaGroups, "KeyID", "Name", "CreatedDate", "State", "EffectiveDate", "ExpirationDate");
        }

        [WebMethod]
        public static string GetFormulaGroupsPendingExpirationWithDataGridFormat(string organizationId)
        {
            DataTable formulaGroups = ExpressionService.GetFormulaGroupsPendingExpiration(organizationId);

            return DataGridJsonParser.DataTableToJson(formulaGroups, "KeyID", "Name", "CreatedDate", "State", "EffectiveDate", "ExpirationDate");
        }

        /// <summary>
        /// 保存报警周期
        /// </summary>
        /// <param name="keyId"></param>
        /// <param name="minutes"></param>
        [WebMethod]
        public static void SaveAlarmPeriod(string keyId, string minutes)
        {
            ExpressionService.SaveAlarmPeriod(new Guid(keyId), int.Parse(minutes));
        }
    }
}