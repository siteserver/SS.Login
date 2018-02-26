using System;
using System.Web.UI.HtmlControls;
using SiteServer.Plugin;
using SS.Login.Core;

namespace SS.Login.Parse
{
    public class StlRegister
    {
        private StlRegister() { }
        public const string ElementName = "stl:register";

        public static string GetApiUrlRegister()
        {
            return LoginPlugin.Instance.PluginApi.GetPluginApiUrl("actions", nameof(Register));
        }

        public static object Register(IRequest request)
        {
            var userName = request.GetPostString("userName");
            var displayName = request.GetPostString("displayName");
            var email = request.GetPostString("email");
            var mobile = request.GetPostString("mobile");
            var password = request.GetPostString("password");

            var userInfo = LoginPlugin.Instance.UserApi.NewInstance();
            userInfo.UserName = userName;
            userInfo.DisplayName = displayName;
            userInfo.Email = email;
            userInfo.Mobile = mobile;

            string errorMessage;
            if (!LoginPlugin.Instance.UserApi.Insert(userInfo, password, out errorMessage))
            {
                throw new Exception(errorMessage);
            }

            return new object();
        }

        public static string Parse(IParseContext context)
        {
            if (!context.StlPageBody.ContainsKey(ParseUtils.GlobalHtmlCodeKey))
            {
                context.StlPageBody.Add(ParseUtils.GlobalHtmlCodeKey, ParseUtils.GetGlobalHtml());
            }

            var stlAnchor = new HtmlAnchor();

            foreach (var name in context.StlAttributes.Keys)
            {
                var value = context.StlAttributes[name];
                stlAnchor.Attributes.Add(name, value);
            }

            stlAnchor.InnerHtml = LoginPlugin.Instance.ParseApi.ParseInnerXml(context.StlInnerXml, context);
            stlAnchor.HRef = "javascript:;";
            stlAnchor.Attributes.Add("onclick", ParseUtils.OnClickRegister);

            return Utils.GetControlRenderHtml(stlAnchor);
        }
    }
}
