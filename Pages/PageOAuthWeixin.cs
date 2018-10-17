using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SS.Login.Core;
using SS.Login.Models;

namespace SS.Login.Pages
{
    public class PageOAuthWeixin : Page
    {
        public DropDownList DdlIsEnabled;
        public PlaceHolder PhSettings;
        public TextBox TbAppId;
        public TextBox TbAppSecret;
        public Button BtnReturn;

        private ConfigInfo _configInfo;

        public string GetRedirectUrl()
        {
            return nameof(PageSettings) + ".aspx";
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (!LoginPlugin.Request.AdminPermissions.HasSystemPermissions(LoginPlugin.PluginId))
            {
                HttpContext.Current.Response.Write("<h1>未授权访问</h1>");
                HttpContext.Current.Response.End();
                return;
            }

            _configInfo = Utils.GetConfigInfo();

            if (IsPostBack) return;

            Utils.SelectListItems(DdlIsEnabled, _configInfo.IsWeixin.ToString());

            PhSettings.Visible = _configInfo.IsWeixin;

            TbAppId.Text = _configInfo.WeixinAppId;
            TbAppSecret.Text = _configInfo.WeixinAppSecret;

            BtnReturn.Attributes.Add("onclick", $"location.href='{PageOAuth.GetRedirectUrl()}';return false");
        }

        public void DdlIsEnabled_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSettings.Visible = Convert.ToBoolean(DdlIsEnabled.SelectedValue);
        }

        public void Submit_OnClick(object sender, EventArgs e)
        {
            _configInfo.IsWeixin = Convert.ToBoolean(DdlIsEnabled.SelectedValue);

            _configInfo.WeixinAppId = TbAppId.Text;
            _configInfo.WeixinAppSecret = TbAppSecret.Text;

            LoginPlugin.ConfigApi.SetConfig(LoginPlugin.PluginId, 0, _configInfo);

            Response.Redirect(PageOAuth.GetRedirectUrl());
        }
    }
}