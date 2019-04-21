using System.Web.UI.HtmlControls;
using SiteServer.Plugin;

namespace SS.Login.Core
{
    public class StlLogout
    {
        private StlLogout() { }
        public const string ElementName = "stl:logout";

        public const string AttributeRedirectUrl = "redirectUrl";

        public static string Parse(IParseContext context)
        {
            var redirectUrl = string.Empty;

            ParseUtils.RegisterBodyHtml(context);

            var stlAnchor = new HtmlAnchor();

            foreach (var name in context.StlAttributes.AllKeys)
            {
                var value = context.StlAttributes[name];
                if (Utils.EqualsIgnoreCase(name, AttributeRedirectUrl))
                {
                    redirectUrl = Context.ParseApi.ParseAttributeValue(value, context);
                }
                else
                {
                    stlAnchor.Attributes.Add(name, value);
                }
            }

            if (string.IsNullOrEmpty(redirectUrl))
            {
                redirectUrl = Context.ParseApi.GetCurrentUrl(context);
            }

            stlAnchor.InnerHtml = Context.ParseApi.Parse(context.StlInnerHtml, context);
            stlAnchor.HRef = "javascript:;";
            stlAnchor.Attributes.Add("onclick", ParseUtils.OnClickLogout);

            return Utils.GetControlRenderHtml(stlAnchor);
        }

        //public static object Logout(IRequest request)
        //{
        //    request.UserLogout();
        //    return new object();
        //}
    }
}
