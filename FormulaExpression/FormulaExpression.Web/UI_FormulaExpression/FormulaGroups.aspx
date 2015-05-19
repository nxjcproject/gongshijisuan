<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FormulaGroups.aspx.cs" Inherits="FormulaExpression.Web.UI_FormulaExpression.FormulaGroups" %>
<%@ Register Src="/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagName="OrganisationTree" TagPrefix="uc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>公式组列表</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css"/>
	<link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css"/>

	<script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
	<script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>
</head>
<body class="easyui-layout">
    <form id="form1" runat="server">
    <div data-options="region:'west',split:false" style="width:230px">
        <uc1:OrganisationTree ID="OrganisationTree_ProductionLine" runat="server" />
    </div>
	<div data-options="region:'east',split:true,title:'公式组使用状态'" style="width:40%;padding:2px;">
	    <table id="formulaGroupsEffectived" class="easyui-datagrid" title="正在使用中的公式组" style="width:100%;height:33%"
			    data-options="singleSelect:true,collapsible:true">
		    <thead>
			    <tr>
				    <th data-options="field:'KeyID',hidden:true"></th>
				    <th data-options="field:'Name',width:200">公式组名称</th>
				    <th data-options="field:'EffectiveDate',width:80">生效时间</th>
				    <th data-options="field:'ExpirationDate',width:80">失效时间</th>
				    <th data-options="field:'attr1',width:40,formatter:formatAction"">查看</th>
			    </tr>
		    </thead>
	    </table>
	    <table id="formulaGroupsPendingEffectived" class="easyui-datagrid" title="即将启用的公式组" style="width:100%;height:33%"
			    data-options="singleSelect:true,collapsible:true,collapsed:false">
		    <thead>
			    <tr>
				    <th data-options="field:'KeyID',hidden:true">Item ID</th>
				    <th data-options="field:'Name',width:200">公式组名称</th>
				    <th data-options="field:'EffectiveDate',width:80">生效时间</th>
				    <th data-options="field:'ExpirationDate',width:80">失效时间</th>
				    <th data-options="field:'attr1',width:40,formatter:formatAction"">查看</th>
			    </tr>
		    </thead>
	    </table>
	    <table id="formulaGroupsPendingExpiration" class="easyui-datagrid" title="即将停用的公式组" style="width:100%;height:33%"
			    data-options="singleSelect:true,collapsible:true,collapsed:false">
		    <thead>
			    <tr>
				    <th data-options="field:'KeyID',hidden:true">Item ID</th>
				    <th data-options="field:'Name',width:200">公式组名称</th>
				    <th data-options="field:'EffectiveDate',width:80">生效时间</th>
				    <th data-options="field:'ExpirationDate',width:80">失效时间</th>
				    <th data-options="field:'attr1',width:40,formatter:formatAction"">查看</th>
			    </tr>
		    </thead>
	    </table>
	</div>
    <div data-options="region:'center'" style="padding:2px;">
	    <table id="formulaGroups" class="easyui-datagrid" title="公式组列表" style="width:100%;height:100%"
			    data-options="rownumbers:true,singleSelect:true,toolbar:toolbar">
		    <thead>
			    <tr>
				    <th data-options="field:'KeyID',hidden:true"></th>
				    <th data-options="field:'Name',width:260">公式组名称</th>
				    <th data-options="field:'CreatedDate',width:120">创建时间</th>
				    <th data-options="field:'State',width:80,formatter:formatState">状态</th>
                    <th data-options="field:'unitcost',width:80,formatter:formatAction">操作</th>
			    </tr>
		    </thead>
	    </table>
    </div>
	<script type="text/javascript">
	    var toolbar = [
        <% if(CanAdd) { %>
        {
	        text: '新增公式组',
	        iconCls: 'icon-add',
	        handler: createNewFormulaGroup
        }
        <% }%>
        <% if(CanAdd && CanDelete) { %>
        , '-',
        <% }%>
        <% if(CanDelete) { %>
        {
	        text: '删除公式组',
	        iconCls: 'icon-clear',
	        handler: function () { alert('save') }
        }
        <% }%>
	    ];

	    function formatState(val, row) {
	        return '暂存';
	    }
	    function formatAction(val, row) {
            <% if(CanEdit) { %>
	        return '<a href="ProductLineEnergyConsumptionAndAlarmParaSetting.aspx?organizationId=' + organizationId + '&keyId=' + row.KeyID + '">编辑</a>';
	        <% } else { %>
	        return '无操作';
            <% }%>
	    }

        // 当前所选组织机构ID
	    var organizationId;

	    function onOrganisationTreeClick(node) {
	        organizationId = node.OrganizationId;
	        loadFormulaGroups();
	        loadFormulaGroupsEffectived();
	        loadFormulaGroupsPendingEffectived();
	        loadFormulaGroupsPendingExpiration();
	    }  

	    // 所有公式组
	    function loadFormulaGroups() {
	        var queryUrl = 'FormulaGroups.aspx/GetFormulaGroupsWithDataGridFormat';
	        var dataToSend = '{organizationId: "' + organizationId + '"}';

	        $.ajax({
	            type: "POST",
	            url: queryUrl,
	            data: dataToSend,
	            contentType: "application/json; charset=utf-8",
	            dataType: "json",
	            success: function (msg) {
	                initializeFormulaGroups(jQuery.parseJSON(msg.d));
	            }
	        });
	    }

	    function initializeFormulaGroups(jsonData) {
	        $('#formulaGroups').datagrid({
	            data: jsonData,
	            dataType: "json"
	        });
	    }

	    // 生效中的公式组
	    function loadFormulaGroupsEffectived() {
	        var queryUrl = 'FormulaGroups.aspx/GetFormulaGroupsEffectivedWithDataGridFormat';
	        var dataToSend = '{organizationId: "' + organizationId + '"}';

	        $.ajax({
	            type: "POST",
	            url: queryUrl,
	            data: dataToSend,
	            contentType: "application/json; charset=utf-8",
	            dataType: "json",
	            success: function (msg) {
	                initializeFormulaGroupsEffectived(jQuery.parseJSON(msg.d));
	            }
	        });
	    }

	    function initializeFormulaGroupsEffectived(jsonData) {
	        $('#formulaGroupsEffectived').datagrid({
	            data: jsonData,
	            dataType: "json"
	        });
	    }

	    // 即将生效的公式组
	    function loadFormulaGroupsPendingEffectived() {
	        var queryUrl = 'FormulaGroups.aspx/GetFormulaGroupsPendingEffectivedWithDataGridFormat';
	        var dataToSend = '{organizationId: "' + organizationId + '"}';

	        $.ajax({
	            type: "POST",
	            url: queryUrl,
	            data: dataToSend,
	            contentType: "application/json; charset=utf-8",
	            dataType: "json",
	            success: function (msg) {
	                initializeFormulaGroupsPendingEffectived(jQuery.parseJSON(msg.d));
	            }
	        });
	    }

	    function initializeFormulaGroupsPendingEffectived(jsonData) {
	        $('#formulaGroupsPendingEffectived').datagrid({
	            data: jsonData,
	            dataType: "json"
	        });
	    }

	    // 即将失效的公式组
	    function loadFormulaGroupsPendingExpiration() {
	        var queryUrl = 'FormulaGroups.aspx/GetFormulaGroupsPendingExpirationWithDataGridFormat';
	        var dataToSend = '{organizationId: "' + organizationId + '"}';

	        $.ajax({
	            type: "POST",
	            url: queryUrl,
	            data: dataToSend,
	            contentType: "application/json; charset=utf-8",
	            dataType: "json",
	            success: function (msg) {
	                initializeFormulaGroupsPendingExpiration(jQuery.parseJSON(msg.d));
	            }
	        });
	    }

	    function initializeFormulaGroupsPendingExpiration(jsonData) {
	        $('#formulaGroupsPendingExpiration').datagrid({
	            data: jsonData,
	            dataType: "json"
	        });
	    }
        
        <% if(CanAdd) {%>
	    // 创建新公式组
	    function createNewFormulaGroup() {
	        var queryUrl = 'FormulaGroups.aspx/CreateFormulaGroup';
	        var dataToSend = '{organizationId: "' + organizationId + '"}';

	        $.ajax({
	            type: "POST",
	            url: queryUrl,
	            data: dataToSend,
	            contentType: "application/json; charset=utf-8",
	            dataType: "json",
	            success: function (msg) {
	                self.location = 'ProductLineEnergyConsumptionAndAlarmParaSetting.aspx?organizationId=' + organizationId + '&keyId=' + jQuery.parseJSON(msg.d).keyId;
	            }
	        });
	    }
        <% }%>
	</script>
    </form>
</body>
</html>
