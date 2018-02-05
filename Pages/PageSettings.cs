using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SiteServer.Plugin;
using SS.Login.Core;
using SS.Login.Model;

namespace SS.Login.Pages
{
    public class PageSettings : Page
    {
        public Literal LtlMessage;
        public CheckBoxList CblRegisterFields;
        public TextBox TbRegisterSuccessMessage;

        private ConfigInfo _configInfo;

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

            CblRegisterFields.Items.Add(new ListItem("用户名", nameof(IUserInfo.UserName)));
            CblRegisterFields.Items.Add(new ListItem("姓名", nameof(IUserInfo.DisplayName)));
            CblRegisterFields.Items.Add(new ListItem("电子邮箱", nameof(IUserInfo.Email)));
            CblRegisterFields.Items.Add(new ListItem("手机号码", nameof(IUserInfo.Mobile)));

            if (_configInfo.RegisterFields == null || _configInfo.RegisterFields.Count == 0)
            {
                _configInfo.RegisterFields = new List<string>
                {
                    nameof(IUserInfo.UserName)
                };
            }
            Utils.SelectListItems(CblRegisterFields, _configInfo.RegisterFields.ToArray());

            TbRegisterSuccessMessage.Text = _configInfo.RegisterSuccessMessage;
        }

        public void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var registerFields = Utils.GetSelectedListControlValueList(CblRegisterFields);
            if (registerFields.Count == 0)
            {
                LtlMessage.Text = Utils.GetMessageHtml("请选择用户注册填写项！", false);
                return;
            }
            if (!registerFields.Contains(nameof(IUserInfo.UserName)) && !registerFields.Contains(nameof(IUserInfo.Email)) && !registerFields.Contains(nameof(IUserInfo.Mobile)))
            {
                LtlMessage.Text = Utils.GetMessageHtml("注册填写项至少需要包含用户名、电子邮箱或者手机号码中的一项！", false);
                return;
            }

            _configInfo.RegisterFields = registerFields;
            _configInfo.RegisterSuccessMessage = TbRegisterSuccessMessage.Text;

            Main.Instance.ConfigApi.SetConfig(0, _configInfo);

            LtlMessage.Text = Utils.GetMessageHtml("用户中心设置修改成功！", true);
        }
    }
}