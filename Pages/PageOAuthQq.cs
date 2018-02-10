using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SS.Login.Core;
using SS.Login.Models;

namespace SS.Login.Pages
{
    public class PageOAuthQq : Page
    {
        public DropDownList DdlIsEnabled;
        public PlaceHolder PhSettings;
        public TextBox TbAppId;
        public TextBox TbAppKey;
        public Button BtnReturn;

        private ConfigInfo _configInfo;

        public string GetRedirectUrl()
        {
            return LoginPlugin.Instance.PluginApi.GetPluginUrl(nameof(PageSettings) + ".aspx");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (!LoginPlugin.Instance.AdminApi.IsPluginAuthorized)
            {
                HttpContext.Current.Response.Write("<h1>未授权访问</h1>");
                HttpContext.Current.Response.End();
                return;
            }

            _configInfo = Utils.GetConfigInfo();

            if (IsPostBack) return;

            Utils.SelectListItems(DdlIsEnabled, _configInfo.IsQq.ToString());

            PhSettings.Visible = _configInfo.IsQq;

            TbAppId.Text = _configInfo.QqAppId;
            TbAppKey.Text = _configInfo.QqAppKey;

            BtnReturn.Attributes.Add("onclick", $"location.href='{PageOAuth.GetRedirectUrl()}';return false");
        }

        public void DdlIsEnabled_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSettings.Visible = Convert.ToBoolean(DdlIsEnabled.SelectedValue);
        }

        public void Submit_OnClick(object sender, EventArgs e)
        {
            _configInfo.IsQq = Convert.ToBoolean(DdlIsEnabled.SelectedValue);

            _configInfo.QqAppId = TbAppId.Text;
            _configInfo.QqAppKey = TbAppKey.Text;

            LoginPlugin.Instance.ConfigApi.SetConfig(0, _configInfo);

            Response.Redirect(PageOAuth.GetRedirectUrl());
        }
    }
}