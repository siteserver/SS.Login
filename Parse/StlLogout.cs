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
            return Main.Instance.PluginApi.GetPluginApiUrl("actions", nameof(Logout));
        }

        public static object Logout(IRequest request)
        {
            request.UserLogout();
            return new object();
        }

        public static string Parse(IParseContext context)
        {
            var redirectUrl = string.Empty;

            if (!context.BodyCodes.ContainsKey(ParseUtils.GlobalHtmlCodeKey))
            {
                context.BodyCodes.Add(ParseUtils.GlobalHtmlCodeKey, ParseUtils.GetGlobalHtml());
            }

            var stlAnchor = new HtmlAnchor();

            foreach (var name in context.StlElementAttributes.Keys)
            {
                var value = context.StlElementAttributes[name];
                if (Utils.EqualsIgnoreCase(name, AttributeRedirectUrl))
                {
                    redirectUrl = Main.Instance.ParseApi.ParseAttributeValue(value, context);
                }
                else
                {
                    stlAnchor.Attributes.Add(name, value);
                }
            }

            if (string.IsNullOrEmpty(redirectUrl))
            {
                redirectUrl = Main.Instance.ParseApi.GetCurrentUrl(context);
            }

            stlAnchor.InnerHtml = Main.Instance.ParseApi.ParseInnerXml(context.StlElementInnerXml, context);
            stlAnchor.HRef = "javascript:;";
            stlAnchor.Attributes.Add("onclick", ParseUtils.OnClickLogout);

            return Utils.GetControlRenderHtml(stlAnchor);
        }
    }
}
