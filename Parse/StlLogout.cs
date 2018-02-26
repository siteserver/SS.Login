using System.Web.UI.HtmlControls;
using SiteServer.Plugin;
using SS.Login.Core;

namespace SS.Login.Parse
{
    public class StlLogout
    {
        private StlLogout() { }
        public const string ElementName = "stl:logout";

        public const string AttributeRedirectUrl = "redirectUrl";

        public static string GetApiUrlLogout()
        {
            return LoginPlugin.Instance.PluginApi.GetPluginApiUrl("actions", nameof(Logout));
        }

        public static object Logout(IRequest request)
        {
            request.UserLogout();
            return new object();
        }

        public static string Parse(IParseContext context)
        {
            var redirectUrl = string.Empty;

            if (!context.StlPageBody.ContainsKey(ParseUtils.GlobalHtmlCodeKey))
            {
                context.StlPageBody.Add(ParseUtils.GlobalHtmlCodeKey, ParseUtils.GetGlobalHtml());
            }

            var stlAnchor = new HtmlAnchor();

            foreach (var name in context.StlAttributes.Keys)
            {
                var value = context.StlAttributes[name];
                if (Utils.EqualsIgnoreCase(name, AttributeRedirectUrl))
                {
                    redirectUrl = LoginPlugin.Instance.ParseApi.ParseAttributeValue(value, context);
                }
                else
                {
                    stlAnchor.Attributes.Add(name, value);
                }
            }

            if (string.IsNullOrEmpty(redirectUrl))
            {
                redirectUrl = LoginPlugin.Instance.ParseApi.GetCurrentUrl(context);
            }

            stlAnchor.InnerHtml = LoginPlugin.Instance.ParseApi.ParseInnerXml(context.StlInnerXml, context);
            stlAnchor.HRef = "javascript:;";
            stlAnchor.Attributes.Add("onclick", ParseUtils.OnClickLogout);

            return Utils.GetControlRenderHtml(stlAnchor);
        }
    }
}
