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
	<script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
	    <h2>Editable TreeGrid</h2>
	    <p>Select one node and click edit button to perform editing.</p>
	    <div style="margin:20px 0;">
		    <a href="javascript:void(0)" class="easyui-linkbutton" onclick="edit()">Edit</a>
		    <a href="javascript:void(0)" class="easyui-linkbutton" onclick="save()">Save</a>
		    <a href="javascript:void(0)" class="easyui-linkbutton" onclick="cancel()">Cancel</a>
	    </div>
	    <table id="tg" class="easyui-treegrid" title="Editable TreeGrid" style="width:700px;height:250px"
			    data-options="
				    iconCls: 'icon-ok',
				    rownumbers: true,
				    animate: true,
				    collapsible: true,
				    fitColumns: true,
				    url: 'treegrid_data2.htm',
				    method: 'get',
				    idField: 'id',
				    treeField: 'name',
				    showFooter: true,
				    onClickCell: clickCell,
				    onDblClickCell: dblClickCell
			    ">
		    <thead>
			    <tr>
				    <th data-options="field:'name',width:180,editor:'text'">Task Name</th>
				    <th data-options="field:'persons',width:60,align:'right',editor:'numberbox'">Persons</th>
				    <th data-options="field:'begin',width:80,editor:'datebox'">Begin Date</th>
				    <th data-options="field:'end',width:80,editor:'datebox'">End Date</th>
				    <th data-options="field:'progress',width:120,formatter:formatProgress,editor:'numberbox'">Progress</th>
			    </tr>
		    </thead>
	    </table>
	    <!--
	    <table id="tg" class="easyui-treegrid" title="公式录入" style="width:100%;height:450px"
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
				    <th data-options="field:'Name',width:180,editor:'text'">工序名称</th>
				    <th data-options="field:'Formula',width:180,formatter:formatFormula,editor:'text'">公式</th>
			    </tr>
		    </thead>
	    </table>
	    -->
	    <div id="formulaEditor" class="easyui-window" title="电量公式编辑" data-options="closed:true,collapsible:false,minimizable:false,resizable:false,iconCls:'icon-sum'" style="width:800px;height:120px;padding:10px;">
		    <input id="formulaEditor_textbox" class="easyui-textbox" style="width:100%" />
		    <a href="#" class="easyui-linkbutton" style="float:right;margin-top:10px;" data-options="iconCls:'icon-cancel'" onclick="$('#formulaEditor').window('close');">取消</a>
		    <a id="formulaEditorSave" href="#" class="easyui-linkbutton" style="float:right;margin-top:10px;margin-right:10px;" data-options="iconCls:'icon-ok'" onclick="formulaEditorSave()">保存</a>
	    </div>
	
	    <script type="text/javascript">
	        function formatProgress(value) {
	            if (value) {
	                var s = '<div style="width:100%;border:1px solid #ccc">' +
		    			    '<div style="width:' + value + '%;background:#cc0000;color:#fff">' + value + '%' + '</div>'
	                '</div>';
	                return s;
	            } else {
	                return '';
	            }
	        }
	        // 当前编辑行ID
	        var editingId;

	        // 编辑
	        function edit() {
	            if (editingId != undefined) {
	                $('#tg').treegrid('select', editingId);
	                return;
	            }
	            var row = $('#tg').treegrid('getSelected');
	            if (row) {
	                editingId = row.id
	                $('#tg').treegrid('beginEdit', editingId);
	                // 为编辑框添加单击事件
	                amountEditorsClickFunction();
	            }
	        }

	        // 保存
	        function save() {
	            if (editingId != undefined) {
	                var t = $('#tg');
	                t.treegrid('endEdit', editingId);
	                editingId = undefined;
	                var persons = 0;
	                var rows = t.treegrid('getChildren');
	                for (var i = 0; i < rows.length; i++) {
	                    var p = parseInt(rows[i].persons);
	                    if (!isNaN(p)) {
	                        persons += p;
	                    }
	                }
	                var frow = t.treegrid('getFooterRows')[0];
	                frow.persons = persons;
	                t.treegrid('reloadFooter');
	            }
	        }

	        // 取消编辑
	        function cancel() {
	            if (editingId != undefined) {
	                $('#tg').treegrid('cancelEdit', editingId);
	                editingId = undefined;
	            }
	        }

	        // 挂载编辑器单击事件
	        function amountEditorsClickFunction() {
	            // 电量公式列
	            var editor = $('#tg').treegrid('getEditor', {
	                id: editingId,
	                field: 'name'
	            });
	            if (editor != null) {
	                editor.target[0].readOnly = true;
	                editor.target[0].onfocus = openFormulaEditorWindow;
	            }
	        }

	        // 弹出公式编辑窗口
	        function openFormulaEditorWindow() {
	            var editor = $('#tg').treegrid('getEditor', {
	                id: editingId,
	                field: 'name'
	            });
	            $('#formulaEditor_textbox').textbox('setText', editor.target.val());
	            $('#formulaEditor').window('open');
	        }

	        // 公式编辑确定
	        function formulaEditorSave() {
	            var editor = $('#tg').treegrid('getEditor', {
	                id: editingId,
	                field: 'name'
	            });
	            editor.target.val($('#formulaEditor_textbox').textbox('getText'));
	            $('#formulaEditor').window('close');
	        }

	        // tg单击处理
	        function clickCell(field, row) {
	            // 如果不是当前编辑行，取消编辑状态
	            save();
	            $('#formulaEditor').window('close');
	        }

	        // tg双击处理
	        function dblClickCell(field, row) {
	            // 首先进入编辑模式
	            edit();
	            // 如果字段为电量公式，则弹出公式编辑窗口
	            if (field == "name")
	                openFormulaEditorWindow();
	            // 如果字段为电耗公式（分母），则弹出电耗公式编辑窗口
	            // 如果字段为相关参数，则弹出相关参数编辑窗口
	        }
	    </script>

    </div>
    </form>
</body>
</html>
