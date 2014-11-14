<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductLineEnergyConsumptionAndAlarmParaSetting.aspx.cs" Inherits="FormulaExpression.Web.UI_FormulaExpression.ProductLineEnergyConsumptionAndAlarmParaSetting" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>生产线能耗公式与报警参数设置</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css"/>
	<link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css"/>

	<script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/jquery.utility.js"></script>
	<script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="wrapper" class="easyui-panel" style="width:98%;height:auto;padding:2px;">

	    <div class="easyui-panel" style="width:100%;padding:5px;">
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-back'" onclick="javascript:history.go(-1);">返回</a> | 
		    名称：<input id="formulaGroupName" class="easyui-validatebox textbox" data-options="required:true,validType:'length[3,50]'" /> | 

            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-save'" onclick="temporarySave()">暂存</a> 
            <a href="javascript:void(0)" class="easyui-linkbutton c4 easyui-tooltip tooltip-f" data-options="plain:true,iconCls:'icon-ok'" title="提交后不可修改，请谨慎操作。" onclick="">提交</a>
	    </div>

        <div class="easyui-panel" style="width:100%;padding:5px;margin-top:5px;margin-bottom:5px;">
            报警周期：
            <select id="cc" class="easyui-combobox" name="dept" style="width:100px;">
                <option value="5">&nbsp;5 分钟</option>
                <option value="10">10 分钟</option>
                <option value="20">20 分钟</option>
                <option value="30">30 分钟</option>
                <option value="45">45 分钟</option>
                <option value="60">&nbsp;1 小时</option>
                <option value="120">&nbsp;2 小时</option>
                <option value="240">&nbsp;4 小时</option>
                <option value="480">&nbsp;8 小时</option>
            </select>
            熟料实物煤耗报警值（kg/t）：
            <input id="Text1" class="easyui-validatebox textbox" data-options="required:true"  style="width:100px;"/>
            相关参数: 
            <input id="Text2" class="easyui-validatebox textbox" data-options="required:true"  style="width:220px;"/>
        </div>

	    <table id="tgformulaEditor" class="easyui-treegrid" title="公式录入" style="width:100%;height:450px"
			    data-options="
				    iconCls: 'icon-edit',
				    rownumbers: true,
				    animate: true,
				    collapsible: true,
				    fitColumns: true,
				    idField: 'LevelCode',
				    treeField: 'Name',
				    onContextMenu: onContextMenu,
				    onClickCell: clickCell,
				    onDblClickCell: dblClickCell
			    ">
		    <thead>
			    <tr>
                    <th data-options="field:'LevelCode',width:50">层次码</th>
				    <th data-options="field:'Name',width:100,editor:'text'">工序设备名称</th>
				    <th data-options="field:'Formula',width:100,formatter:formatFormula,editor:'text'">电量公式</th>
                    <th data-options="field:'Denominator',width:100,formatter:formatFormula,editor:'text'">电耗公式（分母）</th>
                    <th data-options="field:'Required',hidden:true">必选</th>
                    <th data-options="field:'AlarmType',width:50,editor:'text'">报警类型</th>
                    <th data-options="field:'EnergyAlarmValue',width:50,editor:'text'">能耗报警值</th>
                    <th data-options="field:'PowerAlarmValue',width:50,editor:'text'">功率报警值</th>
                    <th data-options="field:'RelativeParameters',width:100,editor:'text'">相关参数</th>
                    <th data-options="field:'Remarks',width:100,editor:'text'">备注</th>
			    </tr>
		    </thead>
	    </table>

	    <div class="easyui-panel" style="width:100%;padding:5px;margin-top:5px;">
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-add'" onclick="appendRoot()">添加根工序</a> | 
            <a href="javascript:void(0)" class="easyui-linkbutton easyui-tooltip tooltip-f" data-options="plain:true,iconCls:'icon-reload'" title="从其他公式组载入。" onclick="$('#dlgLoad').dialog('open')">载入</a> | 
            <a href="javascript:void(0)" class="easyui-linkbutton easyui-tooltip tooltip-f" data-options="plain:true,iconCls:'icon-filter'" title="查看可用的电表变量。" onclick="$('#dlgAmmeter').dialog('open')">电表变量表</a> 
            <a href="javascript:void(0)" class="easyui-linkbutton easyui-tooltip tooltip-f" data-options="plain:true,iconCls:'icon-filter'" title="查看可用的累积量变量。" onclick="$('#dlgCumulant').dialog('open')">累计量变量表</a> 
	    </div>

        <!-- 右键菜单开始 -->
	    <div id="mm" class="easyui-menu" style="width:120px;">
		    <div onclick="append()" data-options="iconCls:'icon-add'">添加</div>
		    <div onclick="removeIt()" data-options="iconCls:'icon-remove'">删除</div>
		    <div class="menu-sep"></div>
            <div onclick="appendRoot()" data-options="iconCls:'icon-add'">添加根工序</div>
            <div class="menu-sep"></div>
		    <div onclick="collapse()">收起</div>
		    <div onclick="expand()">展开</div>
	    </div>
        <!-- 右键菜单结束 -->

        <!-- 弹窗编辑开始 -->
	    <div id="formulaEditor" class="easyui-window" title="电量公式编辑" data-options="closed:true,collapsible:false,minimizable:false,resizable:false,iconCls:'icon-sum'" style="width:800px;height:120px;padding:10px;">
		    <input id="formulaEditor_textbox" class="easyui-textbox" style="width:100%" />
		    <a href="#" class="easyui-linkbutton" style="float:right;margin-top:10px;" data-options="iconCls:'icon-cancel'" onclick="$('#formulaEditor').window('close');">取消</a>
		    <a id="formulaEditorSave" href="#" class="easyui-linkbutton" style="float:right;margin-top:10px;margin-right:10px;" data-options="iconCls:'icon-ok'" onclick="formulaEditorSave()">保存</a>
	    </div>

	    <div id="denominatorEditor" class="easyui-window" title="电耗公式（分母）编辑" data-options="closed:true,collapsible:false,minimizable:false,resizable:false,iconCls:'icon-sum'" style="width:800px;height:120px;padding:10px;">
		    <input id="denominatorEditor_textbox" class="easyui-textbox" style="width:100%" />
		    <a href="#" class="easyui-linkbutton" style="float:right;margin-top:10px;" data-options="iconCls:'icon-cancel'" onclick="$('#denominatorEditor').window('close');">取消</a>
		    <a id="denominatorEditorSave" href="#" class="easyui-linkbutton" style="float:right;margin-top:10px;margin-right:10px;" data-options="iconCls:'icon-ok'" onclick="denominatorEditorSave()">保存</a>
	    </div>
        <!-- 弹窗编辑结束 -->

        <!-- 载入公式组窗口开始 -->
        <div id="dlgLoad" class="easyui-dialog" title="载入公式组" style="width:500px;height:500px;" 
            data-options="
                iconCls:'icon-reload',
                modal:true,
                closed:true
            " >
	        <table id="formulaGroups" class="easyui-datagrid" style="width:100%;height:100%"
			        data-options="singleSelect:true">
		        <thead>
			        <tr>
				        <th data-options="field:'KeyID',hidden:true"></th>
				        <th data-options="field:'Name',width:260">公式组名称</th>
				        <th data-options="field:'CreateDate',width:100">创建时间</th>
				        <th data-options="field:'State',width:100,align:'center'">状态</th>
			        </tr>
		        </thead>
	        </table>
	    </div>
        <!-- 载入公式组窗口结束 -->

        <!-- 电表变量表窗口开始 -->
        <div id="dlgAmmeter" class="easyui-dialog" title="电表变量表" style="width:550px;height:500px;" 
            data-options="
                iconCls:'icon-filter',
                closed:true
            " >
	        <table id="tgAmmeters" class="easyui-treegrid" style="width:100%;height:100%"
			        data-options="idField:'AmmeterNumber',treeField:'AmmeterNumber',singleSelect:true">
		        <thead>
			        <tr>
				        <th data-options="field:'AmmeterNumber',width:260">电表变量</th>
				        <th data-options="field:'AmmeterName',width:260">电表描述</th>
			        </tr>
		        </thead>
	        </table>
	    </div>
        <!-- 电表变量表窗口结束 -->

        <!-- 累计量变量表窗口开始 -->
        <div id="dlgCumulant" class="easyui-dialog" title="累计量变量表" style="width:500px;height:500px;" 
            data-options="
                iconCls:'icon-reload',
                modal:true,
                closed:true
            " >
	        <table id="Table2" class="easyui-datagrid" style="width:100%;height:100%"
			        data-options="singleSelect:true">
		        <thead>
			        <tr>
				        <th data-options="field:'KeyID',hidden:true"></th>
				        <th data-options="field:'Name',width:260">公式组名称</th>
				        <th data-options="field:'CreateDate',width:100">创建时间</th>
				        <th data-options="field:'State',width:100,align:'center'">状态</th>
			        </tr>
		        </thead>
	        </table>
	    </div>
        <!-- 累计量变量表窗口结束 -->

	    <script type="text/javascript">

	        // 公式检验样式
	        function formatFormula(value) {
	            var validateResult = validateExpression(value);

	            if (validateResult == "success") {
	                var s = '<div style="width:100%;" title="' + value + '">' + value;
	                s += '<div style="float:right;width:16px;height:16px;" class="icon-ok"></div></div>';
	                return s;
	            }
	            else {
	                var s = '<div style="width:100%;">' + value;
	                s += '<div title="' + validateResult + '" class="easyui-tooltip icon-no" style="float:right;width:16px;height:16px;"></div></div>';
	                return s;
	            }
	        }

	        //////////////////////////////////////////////////////////////////////

	        // 验证公式合法性
	        function validateExpression(expression) {
	            var queryUrl = 'ProductLineEnergyConsumptionAndAlarmParaSetting.aspx/ValidateExpression';
	            var dataToSend = '{expression: "' + expression + '"}';
	            var result = '验证服务不可用';

	            $.ajax({
	                type: "POST",
	                url: queryUrl,
	                data: dataToSend,
	                async: false,
	                contentType: "application/json; charset=utf-8",
	                dataType: "json",
	                success: function (msg) {
	                    result = msg.d;
	                }
	            });

	            return result;
	        }

	        //////////////////////////////////////////////////////////////////////

	        // 右键菜单
	        function onContextMenu(e, row) {
	            e.preventDefault();
	            $(this).treegrid('select', row.LevelCode);
	            $('#mm').menu('show', {
	                left: e.pageX,
	                top: e.pageY
	            });
	        }

	        // 添加根节点
	        function appendRoot() {
	            var levelCode = getAppendRootLevelCode();
	            $('#tgformulaEditor').treegrid('append', {
	                data: [{
	                    LevelCode: levelCode,
	                    Name: '新工序',
	                    Formula: '',
	                    Denominator: ''
	                }]
	            })
	        }

	        //添加子节点
	        function append() {
	            var node = $('#tgformulaEditor').treegrid('getSelected');
	            var levelCode = getAppendLevelCode(node.LevelCode);
	            $('#tgformulaEditor').treegrid('append', {
	                parent: node.LevelCode,
	                data: [{
	                    LevelCode: levelCode,
	                    Name: '新工序',
	                    Formula: '',
	                    Denominator: ''
	                }]
	            })
	        }

	        // 删除节点
	        function removeIt() {
	            var node = $('#tgformulaEditor').treegrid('getSelected');
                // 不可删除必须的工序
	            if (node.Required) {
	                $.messager.alert('错误', '此项为系统必须，不可删除');
	                return;
	            }
	            if (node) {
	                $('#tgformulaEditor').treegrid('remove', node.LevelCode);
	            }
	        }

	        // 收起节点
	        function collapse() {
	            var node = $('#tgformulaEditor').treegrid('getSelected');
	            if (node) {
	                $('#tgformulaEditor').treegrid('collapse', node.LevelCode);
	            }
	        }

	        // 展开节点
	        function expand() {
	            var node = $('#tgformulaEditor').treegrid('getSelected');
	            if (node) {
	                $('#tgformulaEditor').treegrid('expand', node.LevelCode);
	            }
	        }

	        ////////////////////////////////////////////////////////////////////////////////

	        // 生成根节点ID
	        function getAppendRootLevelCode() {
	            var rows = $('#tgformulaEditor').treegrid('getRoots');
	            if (rows.length == 0) {
	                return 'P01';
	            }
	            else {
	                var maxCode = 0;
	                for (var i = 0; i < rows.length; i++) {
	                    var temp = rows[i].LevelCode;
	                    if (temp.length != 3)
	                        continue;
	                    var p = parseInt(temp.substring(1, temp.length));
	                    if (p > maxCode)
	                        maxCode = p;
	                }
	                maxCode = maxCode + 1;
	                if (maxCode.toString().length % 2 == 1)
	                    return 'P0' + maxCode;
	                else
	                    return 'P' + maxCode;
	            }
	        }

	        // 生成子节点ID
	        function getAppendLevelCode(parentId) {
	            var rows = $('#tgformulaEditor').treegrid('getChildren', parentId);

	            if (rows.length == 0) {
	                return parentId + '01';
	            }
	            else {
	                var maxCode = 0;
	                for (var i = 0; i < rows.length; i++) {
	                    var temp = rows[i].LevelCode;
	                    if (temp.length != parentId.length + 2)
	                        continue;
	                    var p = parseInt(temp.substring(1, temp.length));
	                    if (p > maxCode)
	                        maxCode = p;
	                }
	                maxCode = maxCode + 1;
	                if (maxCode.toString().length % 2 == 1)
	                    return 'P0' + maxCode;
	                else
	                    return 'P' + maxCode;
	            }
	        }

	        ///////////////////////////////////////////////////////////


	        // 当前编辑行ID
	        var editingId;

	        // 编辑
	        function edit() {
	            if (editingId != undefined) {
	                $('#tgformulaEditor').treegrid('select', editingId);
	                return;
	            }
	            var row = $('#tgformulaEditor').treegrid('getSelected');
	            if (row) {
	                editingId = row.LevelCode
	                $('#tgformulaEditor').treegrid('beginEdit', editingId);
	                // 为编辑框添加单击事件
	                amountEditorsClickFunction();
	            }
	        }

	        // 保存
	        function save() {
	            if (editingId != undefined) {
	                var t = $('#tgformulaEditor');
	                t.treegrid('endEdit', editingId);
	                editingId = undefined;
	            }
	        }

	        // 取消编辑
	        function cancel() {
	            if (editingId != undefined) {
	                $('#tgformulaEditor').treegrid('cancelEdit', editingId);
	                editingId = undefined;
	            }
	        }

	        // 挂载编辑器单击事件
	        function amountEditorsClickFunction() {
	            // 电量公式列
	            var editor = $('#tgformulaEditor').treegrid('getEditor', {
	                id: editingId,
	                field: 'Formula'
	            });
	            if (editor != null) {
	                editor.target[0].readOnly = true;
	                editor.target[0].onfocus = openFormulaEditorWindow;
	            }
	            // 电耗公式（分母）列
	            var editor = $('#tgformulaEditor').treegrid('getEditor', {
	                id: editingId,
	                field: 'Denominator'
	            });
	            if (editor != null) {
	                editor.target[0].readOnly = true;
	                editor.target[0].onfocus = openDenominatorEditorWindow;
	            }

	        }

	        // 弹出公式编辑窗口
	        function openFormulaEditorWindow() {
	            var editor = $('#tgformulaEditor').treegrid('getEditor', {
	                id: editingId,
	                field: 'Formula'
	            });
	            $('#formulaEditor_textbox').textbox('setText', editor.target.val());
	            $('#formulaEditor').window('open');
	        }

            // 弹出分母编辑窗口
	        function openDenominatorEditorWindow() {
	            var editor = $('#tgformulaEditor').treegrid('getEditor', {
	                id: editingId,
	                field: 'Denominator'
	            });
	            $('#denominatorEditor_textbox').textbox('setText', editor.target.val());
	            $('#denominatorEditor').window('open');
	        }

	        // 公式编辑确定
	        function formulaEditorSave() {
	            var editor = $('#tgformulaEditor').treegrid('getEditor', {
	                id: editingId,
	                field: 'Formula'
	            });
	            editor.target.val($('#formulaEditor_textbox').textbox('getText'));
	            $('#formulaEditor').window('close');
	        }

            // 分母编辑确定
	        function denominatorEditorSave() {
	            var editor = $('#tgformulaEditor').treegrid('getEditor', {
	                id: editingId,
	                field: 'Denominator'
	            });
	            editor.target.val($('#denominatorEditor_textbox').textbox('getText'));
	            $('#denominatorEditor').window('close');
	        }

	        // tg单击处理
	        function clickCell(field, row) {
	            // 如果不是当前编辑行，取消编辑状态
	            save();
	            $('#formulaEditor').window('close');
	            $('#denominatorEditor').window('close');
	        }

	        // tg双击处理
	        function dblClickCell(field, row) {
	            // 首先进入编辑模式
	            edit();
	            // 如果字段为电量公式，则弹出公式编辑窗口
	            if (field == "Formula")
	                openFormulaEditorWindow();
	            // 如果字段为电耗公式（分母），则弹出电耗公式编辑窗口
	            if (field == "Denominator")
	                openDenominatorEditorWindow();
	            // 如果字段为相关参数，则弹出相关参数编辑窗口
	        }

	        ///////////////////////////////////////////////////////////////////////

	        // 按公式组KeyID获取公式组名称
	        function loadName(keyId) {
	            var queryUrl = 'ProductLineEnergyConsumptionAndAlarmParaSetting.aspx/GetFormulaName';
	            var dataToSend = '{keyId: "' + keyId + '"}';

	            $.ajax({
	                type: "POST",
	                url: queryUrl,
	                data: dataToSend,
	                contentType: "application/json; charset=utf-8",
	                dataType: "json",
	                success: function (msg) {
	                    $('#formulaGroupName').val(jQuery.parseJSON(msg.d).name);
	                }
	            });
	        }

	        // 按公式组获取所有公式
	        function loadFormulas(keyId) {
	            var queryUrl = 'ProductLineEnergyConsumptionAndAlarmParaSetting.aspx/GetFormulasWithTreeGridFormat';
	            var dataToSend = '{keyId: "' + keyId + '"}';

	            var win = $.messager.progress({
	                title: '请稍后',
	                msg: '数据载入中...'
	            });

	            $.ajax({
	                type: "POST",
	                url: queryUrl,
	                data: dataToSend,
	                contentType: "application/json; charset=utf-8",
	                dataType: "json",
	                success: function (msg) {
	                    initializeFormulaEditor(jQuery.parseJSON(msg.d));
	                    $.messager.progress('close');
	                },
	                error: function () {
	                    $.messager.progress('close');
	                    $.messager.alert('错误', '数据载入失败！');
	                }
	            });
	        }

	        // 初始化公式编辑器
	        function initializeFormulaEditor(jsonData) {
	            $('#tgformulaEditor').treegrid({
	                data: jsonData,
	                dataType: "json"
	            });
	        }

	        ///////////////////////////////////////////////////////////////////////

	        // 获取公式组
	        function loadFormulaGroups(organizationId) {
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
	                dataType: "json",
	                onDblClickRow: function (rowIndex, rowData) {
	                    loadFormulas(rowData.KeyID);
	                    $('#dlg').dialog('close');
	                }
	            });
	        }

	        //////////////////////////////////////////////////////////////////////

	        // 读取电表变量表

	        function queryAmmeters(organizationId) {
	            var queryUrl = 'ProductLineEnergyConsumptionAndAlarmParaSetting.aspx/GetAmmeterLabelsWithTreeGridFormat';
	            var dataToSend = '{organizationId: "' + organizationId + '"}';

	            $.ajax({
	                type: "POST",
	                url: queryUrl,
	                data: dataToSend,
	                contentType: "application/json; charset=utf-8",
	                dataType: "json",
	                success: function (msg) {
	                    initializeAmmeterTreeGrid(jQuery.parseJSON(msg.d));
	                }
	            });
	        }

	        function initializeAmmeterTreeGrid(jsonData) {
	            $('#tgAmmeters').treegrid({
	                data: jsonData,
	                dataType: "json"
	            });
	        };

	        ///////////////////////////////////////////////////////////////////////

	        $(document).ready(function () {
	            var keyId = $.getUrlParam('keyId');
	            var organizationId = $.getUrlParam('organizationId');

	            loadName(keyId);
	            loadFormulas(keyId);
	            loadFormulaGroups(organizationId);
	            loadAmmeters(organizationId);
	        });

	    </script>
    </div>
    </form>
</body>
</html>
