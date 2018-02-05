using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SS.Login.Core;
using SS.Login.Model;
using SS.Login.Parse;

namespace SS.Login.Pages
{
    public class PageOAuth : Page
    {
        public Literal LtlWeibo;
        public Literal LtlWeixin;
        public Literal LtlQq;
        public Literal LtlScript;

        private ConfigInfo _configInfo;

        public static string GetRedirectUrl()
        {
            return Main.Instance.PluginApi.GetPluginUrl(nameof(PageOAuth) + ".aspx");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (!Main.Instance.AdminApi.IsPluginAuthorized)
            {
                HttpContext.Current.Response.Write("<h1>未授权访问</h1>");
                HttpContext.Current.Response.End();
                return;
            }

            _configInfo = Utils.GetConfigInfo();

            if (IsPostBack) return;

            LtlWeibo.Text = _configInfo.IsWeibo ? @"<span class=""label label-primary"">已开通</span>" : "未开通";
            if (_configInfo.IsWeibo)
            {
                var url =
                    Main.Instance.PluginApi.GetPluginApiUrl(nameof(StlLogin.OAuth), StlLogin.TypeWeibo) +
                    "?redirectUrl=" + HttpUtility.UrlEncode(_configInfo.HomeUrl);
                LtlWeibo.Text +=
                    $@"<a class=""m-l-10"" href=""{url}"" target=""_blank"" onclick=""event.stopPropagation();"">测试</a>";
            }

            LtlWeixin.Text = _configInfo.IsWeixin ? @"<span class=""label label-primary"">已开通</span>" : "未开通";
            if (_configInfo.IsWeixin)
            {
                var url =
                    Main.Instance.PluginApi.GetPluginApiUrl(nameof(StlLogin.OAuth), StlLogin.TypeWeixin) +
                    "?redirectUrl=" + HttpUtility.UrlEncode(_configInfo.HomeUrl);
                LtlWeixin.Text +=
                    $@"<a class=""m-l-10"" href=""{url}"" target=""_blank"" onclick=""event.stopPropagation();"">测试</a>";
            }

            LtlQq.Text = _configInfo.IsQq ? @"<span class=""label label-primary"">已开通</span>" : "未开通";
            if (_configInfo.IsQq)
            {
                var url =
                    Main.Instance.PluginApi.GetPluginApiUrl(nameof(StlLogin.OAuth), StlLogin.TypeQq) +
                    "?redirectUrl=" + HttpUtility.UrlEncode(_configInfo.HomeUrl);
                LtlQq.Text +=
                    $@"<a class=""m-l-10"" href=""{url}"" target=""_blank"" onclick=""event.stopPropagation();"">测试</a>";
            }
        }
    }
}