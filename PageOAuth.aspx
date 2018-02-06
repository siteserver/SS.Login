<%@ Page Language="C#" Inherits="SS.Login.Pages.PageOAuth" %>

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
        <h4 class="m-t-0 header-title">
          <b>第三方登录设置</b>
        </h4>
        <p class="text-muted font-13 m-b-25">
          如已有渠道参数可直接进行参数填写，如尚未获得参数可交由我们代为申请。
        </p>

        <table class="table table-hover m-0">
          <thead>
            <tr>
              <th>第三方登录渠道</th>
              <th>状态</th>
            </tr>
          </thead>
          <tbody>
            <tr style="cursor: pointer" onclick="location.href='pageOAuthWeibo.aspx'">
              <td>
                <div class="m-t-10 m-b-10">
                  <img src="assets/images/login_weibo.png"> 新浪微博</div>
              </td>
              <td>
                <div class="m-t-10 m-b-10">
                  <asp:Literal id="LtlWeibo" runat="server" />
                </div>
              </td>
            </tr>
            <tr style="cursor: pointer" onclick="location.href='pageOAuthWeixin.aspx'">
              <td>
                <div class="m-t-10 m-b-10">
                  <img src="assets/images/login_weixin.png"> 微信</div>
              </td>
              <td>
                <div class="m-t-10 m-b-10">
                  <asp:Literal id="LtlWeixin" runat="server" />
                </div>
              </td>
            </tr>
            <tr style="cursor: pointer" onclick="location.href='pageOAuthQq.aspx'">
              <td>
                <div class="m-t-10 m-b-10">
                  <img src="assets/images/login_qq.png"> QQ</div>
              </td>
              <td>
                <div class="m-t-10 m-b-10">
                  <asp:Literal id="LtlQq" runat="server" />
                </div>
              </td>
            </tr>
          </tbody>
        </table>


      </div>

      <asp:Literal id="LtlScript" runat="server" />

    </form>
  </body>

  </html>