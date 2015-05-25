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
    public partial class FormulaGroups : WebStyleBaseForEnergy.webStyleBase
    {
        public bool IsAdministrator
        {
            get { return true; }
        }

        public bool CanAdd
        {
            get { return true; }
        }

        public bool CanEdit
        {
            get { return true; }
        }

        public bool CanDelete
        {
            get { return true; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();

            ////////////////////调试用,自定义的数据授权
#if DEBUG
            List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc" };
            AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
            this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                         // 向web用户控件传递数据授权参数
            this.OrganisationTree_ProductionLine.PageName = "FormulaGroups.aspx";                                                       // 向web用户控件传递当前调用的页面名称
            //this.OrganisationTree_ProductionLine.LeveDepth = 5;
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

        [WebMethod]
        public static string CreateFormulaGroup(string organizationId)
        {
            Guid id = ExpressionService.CreateNewFormulaGroup(organizationId);

            return "{\"keyId\":\"" + id.ToString() + "\"}";

        }


        /// <summary>
        /// 删除公式组
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        [WebMethod]
        public static void DeleteFormulaGroup(string groupId)
        {
            Guid id = new Guid(groupId);
            ExpressionService.DisableFormulasByGroupId(id);
        }
    }
}