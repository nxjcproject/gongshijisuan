
$(function () {
    //根据页面大小自适应标题
    /*
    var m_RightWidth = $(window).width() - 490 - 300;
    if(m_RightWidth < 0)
    {
        m_RightWidth = 0;
    }
    $('#titleRight').css("width", m_RightWidth);

    $('#mainLayout').layout('panel', 'north').panel({
        onResize: function (width, height) {
            $('#mainTitle').css("width", width);
            $('#mainMenu').css("width", width);

            var m_IRightWidth = $(window).width() - 490 -300;
            if (m_IRightWidth < 0) {
                m_IRightWidth = 0;
            }
            $('#titleRight').css("width", m_IRightWidth);
            //$('.iframeSubPage').css("width");
        }
    });
    */
    /////////////////////////创建框架tab///////////////////////////
    try {
        mainTabs = $('#mainTabs').tabs({
            fit: true,
            border: false,
            tools: []
        });
    }
    catch (e) {

    }
    $('#mainLayout').layout('hidden', 'west');          //隐藏左边菜单栏
    //加载一级菜单
    LoadMainMenu();
    
    SetLeftMenuTitle();

});
function TestFun() {

    $('#mainLayout').layout('hidden', 'west');
}
function TestFun1() {
    $('#mainLayout').layout('show', 'west');
}
//var aa = false;
function SetLeftMenuTitle()
{
    var m_UserName = $('#HiddenField_UserName').val();
    if (m_UserName != '') {
        $("#leftMenu").panel({ title: "当前用户: " + m_UserName });
    }
    else {
        $("#leftMenu").panel({ title: "二级菜单"});
    }
}
//加载一级菜单
function LoadMainMenu() {
    $.ajax({
        type: "POST",
        url: "Default.aspx/GetFristMenu",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            var m_Data = m_MsgData['FirstMenu'];

            for (var i = 0; i < m_Data.length; i++) {
                if (m_Data[i].ParentNodeId == '0') {
                    var m_DivId = 'FirstMenu' + i.toString();
                    var m_MainButton = CreateButton(m_Data[i]);
                    m_MainButton.css('margin-left',5);
                    var m_TreeButton = $('<div id = ' + m_DivId + ' style="width:auto;"></div>');
                    m_MainButton.appendTo($("#mainMenu"));

                    if (m_Data[i].NodeType == 'MenuNode') {                                        //
                        CreateTreeButton(m_Data, m_Data[i].NodeId, m_TreeButton);           // $('<div id = ' + m_DivId + ' style="width:auto;"></div>');

                        if (m_TreeButton.children('div').length > 0) {
                            m_TreeButton.appendTo($("#mainMenu"));
                            m_MainButton.menubutton({ menu: '#' + m_DivId });
                        }
                        else {
                            m_MainButton.linkbutton({ plain: true });
                        }
                    }
                    else {                                                                                //叶子节点和生成左边目录的情况直接生成linkbutton
                        m_MainButton.linkbutton({ plain: true });
                    }
                }
            }

        },
        error: function (e) {
            alert("数据加载失败!");
        }
    });
}
function CreateTreeButton(myData, myParentNodeId, myRootButton) {

    for (var i = 0; i < myData.length; i++) {
        if (myData[i].ParentNodeId == myParentNodeId) {
            var m_NodeButton = CreateButton(myData[i]);
            m_NodeButton.appendTo(myRootButton);
            if (myData[i].NodeType != 'LMenuNode') {
                var m_ChildrenRootNode = $('<div></div>');
                CreateTreeButton(myData, myData[i].NodeId, m_ChildrenRootNode);

                if (m_ChildrenRootNode.children('div').length > 0) {                      //如果改节点有子节点
                    var m_Span = $('<span></span>').text(m_NodeButton.text());
                    m_NodeButton.empty();
                    m_Span.appendTo(m_NodeButton);
                    m_ChildrenRootNode.appendTo(m_NodeButton);
                }
            }
        }
    }
}
function CreateButton(myNode) {
    var m_MenuObj = null;
    if (myNode.NodeType == 'MenuNode') {                                        //菜单中间点
        m_MenuObj = CreateMenuButton(myNode) 
    } 
    else if (myNode.NodeType == 'LMenuNode') {                                  //左边菜单
        m_MenuObj = CreateLeftTreeButton(myNode)
    }
    else if (myNode.NodeType == 'LeafNode') {                                   //叶子菜单
        m_MenuObj = CreateLeafButton(myNode)
    }
    return m_MenuObj;
}
function CreateMenuButton(myNode) {
    return m_MenuButton = $("<div></div>").text(myNode.NodeContext);
}
function CreateLeafButton(myNode) {
    var m_MenuButton = $("<div></div>").text(myNode.NodeContext);
    m_MenuButton.click(function () {
        $('#mainLayout').layout('hidden', 'west');
        AddTabFrame(myNode.NavigateUrl, myNode.NodeId, myNode.NodeContext, myNode.OpenIconPath);
    });
    return m_MenuButton;
}
function CreateLeftTreeButton(myNode) {
    var m_MenuButton = $("<div></div>").text(myNode.NodeContext);
    m_MenuButton.click(function () {
        $('#mainLayout').layout('show', 'west');
        LoadLeftTreeMenu(myNode.NodeId);
    });
    return m_MenuButton;
}


////////////////加载左边目录树////////////////
function LoadLeftTreeMenu(myParentId)
{
    var mainMenu;
    var mainTabs;
    var treeData;
    $.ajax({
        type: "POST",
        url: "Default.aspx/GetSecondMenu",
        data: "{myParentId:'" + myParentId + "'}",
        parentField: 'ParentId',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            try {
                secondMenu = $('#secondMenu').tree({
                    data: jQuery.parseJSON(msg.d)["LeftTree"],
                    checkbox: false,
                    onClick: function (node) {
                        if (node.attributes == null) {
                            $(this).tree('toggle', node.target);
                        }
                        else if (node.attributes.url) {
                            var src = node.attributes.url;
                            //if (!sy.startWith(node.attributes.url, '/')) {
                            //    src = node.attributes.url;
                            //}
                            if (node.attributes.target && node.attributes.target.length > 0) {
                                window.open(src, node.attributes.target);
                            } else {
                                AddTabFrame(src, node.NodeId, node.text, node.OpenIconCls);
                            }
                        }
                    }
                });
            }
            catch (e) {

            }
        }
    });
}
////////////////////////////增加自页面Tab////////////////////////////
function AddTabFrame(myUrl,myNodeId, myText, myIconCls) {
    var tabs = $('#mainTabs');
    var opts = {
        title: myText,
        closable: true,
        iconCls: myIconCls,
        content: '<iframe src="' + myUrl + '?PageId=' + myNodeId + '" allowTransparency="true" style="border:0; width:100%; height:99.5%;" frameBorder="0"></iframe>',
        border: false,
        fit: true
    };
    if (tabs.tabs('exists', opts.title)) {
        tabs.tabs('select', opts.title);
    } else {
        if (tabs.tabs('exists', 5)) {
            tabs.tabs('close', 1);
            tabs.tabs('add', opts);
        }
        else {
            tabs.tabs('add', opts);
        }
    }
}

function LogoutFun() {
    self.location = "login.aspx";
}
function ChangePasswordFun() {
    AddTabFrame('SystemManage/ChangePassword.aspx', '5CE25714-15AE-490B-947E-13C28BA20316', '修改密码', 'ext-icon-key');
}
