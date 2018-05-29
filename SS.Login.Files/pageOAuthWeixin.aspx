<%@ Page Language="C#" Inherits="SS.Login.Pages.PageOAuthWeixin" %>
  <!DOCTYPE html>
  <html>

  <head>
    <meta charset="utf-8">
    <link href="assets/plugin-utils/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/plugin-utils.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <script src="assets/js/jquery.min.js"></script>
    <script src="assets/js/sweetalert.min.js"></script>
  </head>

  <body>
    <form id="form" runat="server" class="m-l-15 m-r-15 m-t-15">

      <div class="card-box">
        <ul class="nav nav-pills">
          <li class="nav-item">
            <a class="nav-link" href="pageSettings.aspx">授权认证设置</a>
          </li>
          <li class="nav-item active">
            <a class="nav-link" href="pageOAuth.aspx">第三方登录设置</a>
          </li>
        </ul>
      </div>

      <div class="card-box">
        <div class="row">
          <div class="col-lg-10">
            <h4 class="m-t-0 header-title">
              <b>微信登录设置</b>
            </h4>
            <p class="text-muted font-13 m-b-30">
              在此设置微信登录配置
            </p>
          </div>
        </div>

        <div class="form-horizontal">

          <div class="form-group">
            <label class="col-sm-3 control-label">是否启用微信登录</label>
            <div class="col-sm-3">
              <asp:DropDownList id="DdlIsEnabled" AutoPostBack="true" OnSelectedIndexChanged="DdlIsEnabled_SelectedIndexChanged" class="form-control"
                runat="server">
                <asp:ListItem Text="启用" Value="True" Selected="True" />
                <asp:ListItem Text="不启用" Value="False" />
              </asp:DropDownList>
            </div>
            <div class="col-sm-6">
              <span class="help-block"></span>
            </div>
          </div>

          <asp:PlaceHolder id="PhSettings" runat="server">

            <div class="form-group">
              <label class="col-sm-3 control-label">AppID</label>
              <div class="col-sm-3">
                <asp:TextBox id="TbAppId" class="form-control" runat="server"></asp:TextBox>
              </div>
              <div class="col-sm-6 help-block">
                <asp:RequiredFieldValidator ControlToValidate="TbAppId" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbAppId" ValidationExpression="[^']+" ErrorMessage=" *"
                  ForeColor="red" Display="Dynamic" />
                <a href="https://open.weixin.qq.com" target="_blank">微信开放平台</a> - 管理中心 - 网站应用 - 查看
              </div>
            </div>
            <div class="form-group">
              <label class="col-sm-3 control-label">AppSecret</label>
              <div class="col-sm-3">
                <asp:TextBox id="TbAppSecret" class="form-control" runat="server"></asp:TextBox>
              </div>
              <div class="col-sm-6 help-block">
                <asp:RequiredFieldValidator ControlToValidate="TbAppSecret" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbAppSecret" ValidationExpression="[^']+" ErrorMessage=" *"
                  ForeColor="red" Display="Dynamic" />
                <a href="https://open.weixin.qq.com" target="_blank">微信开放平台</a> - 管理中心 - 网站应用 - 查看
              </div>
            </div>

          </asp:PlaceHolder>

          <div class="form-group m-b-0">
            <div class="col-sm-offset-3 col-sm-9">
              <asp:Button class="btn btn-primary" Text="确 定" OnClick="Submit_OnClick" runat="server" />
              <asp:Button class="btn m-l-10" ID="BtnReturn" Text="返 回" runat="server" />
            </div>
          </div>

        </div>
      </div>

    </form>
  </body>

  </html>