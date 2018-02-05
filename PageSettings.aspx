<%@ Page Language="C#" Inherits="SS.OAuth.Pages.PageSettings" %>
  <!DOCTYPE html>
  <html>

  <head>
    <meta charset="utf-8">
    <link href="assets/plugin-utils/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/plugin-utils.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/ionicons.min.css" rel="stylesheet" type="text/css" />
  </head>

  <body>
    <form id="form" runat="server" class="m-l-15 m-r-15 m-t-15">

      <div class="card-box">
        <ul class="nav nav-pills">
          <li class="nav-item active">
            <a class="nav-link" href="pageSettings.aspx">授权认证设置</a>
          </li>
          <li class="nav-item">
            <a class="nav-link" href="pageOAuth.aspx">第三方登录设置</a>
          </li>
        </ul>
      </div>

      <asp:Literal id="LtlMessage" runat="server" />

      <div class="card-box">
        <div class="m-t-0 header-title">
          授权认证设置
        </div>
        <p class="text-muted font-13 m-b-30">
          在此设置授权认证信息
        </p>

        <div class="form-group">
          <label class="col-form-label">
            用户注册填写项
          </label>
          <asp:CheckBoxList id="CblRegisterFields" class="checkbox checkbox-primary" runat="server" />
        </div>

        <div class="form-group">
            <label class="col-form-label">
              用户注册成功提示
              <asp:RequiredFieldValidator ControlToValidate="TbRegisterSuccessMessage" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbRegisterSuccessMessage" ValidationExpression="[^']+" ErrorMessage=" *"
                  ForeColor="red" Display="Dynamic" />
            </label>
            <asp:TextBox id="TbRegisterSuccessMessage" class="form-control" runat="server" />
          </div>

        <hr>

        <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server" />

      </div>

      </div>
    </form>
  </body>

  </html>