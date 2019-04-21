using System;
using SiteServer.Plugin;

namespace SS.Login.Core
{
    public static class ParseUtils
    {
        public const string OnClickLogin = "$authVue.openLoginModal()";
        public const string OnClickLoginAll = "$authVue.openSocialModal()";
        public const string OnClickRegister = "$authVue.openRegisterModal()";
        public const string OnClickLogout = "$authVue.logout()";

        public static void RegisterBodyHtml(IParseContext context)
        {
            context.BodyCodes[GlobalHtmlKey] = GlobalHtml;
        }

        private const string GlobalHtmlKey = "SS.Login.Parse.GlobalHtml";

        private static string GlobalHtml
        {
            get
            {
                var htmlPath = Context.PluginApi.GetPluginPath(Plugin.PluginId, "assets/template.html");
                var assetsUrl = Context.PluginApi.GetPluginUrl(Plugin.PluginId, "assets");
                var html = CacheUtils.Get<string>(htmlPath);
                if (html != null) return html;

                html = Utils.ReadText(htmlPath);
                var startIndex = html.IndexOf("<!-- template start -->", StringComparison.Ordinal);
                var length = html.IndexOf("<!-- template end -->", StringComparison.Ordinal) - startIndex;
                html = html.Substring(startIndex, length);

                html = $@"
<link rel=""stylesheet"" type=""text/css"" href=""{assetsUrl}/cleanslate.min.css"" />
<link rel=""stylesheet"" type=""text/css"" href=""{assetsUrl}/style.css"" />
{html}
<script type=""text/javascript"" src=""{assetsUrl}/js/jquery.min.js""></script>
<script type=""text/javascript"" src=""{assetsUrl}/js/vue-2.1.10.min.js""></script>
<script type=""text/javascript"" src=""{assetsUrl}/js/vee-validate-2.0.3.js""></script>
<script type=""text/javascript"" src=""{assetsUrl}/js/es6-promise.auto.min.js""></script>
<script type=""text/javascript"" src=""{assetsUrl}/js/axios-0.17.1.min.js""></script>
<script type=""text/javascript"">
var authData = {{
    apiUrlRegister: '{ApiUtils.GetActionsRegisterUrl()}',
    apiUrlLogin: '{ApiUtils.GetActionsLoginUrl()}',
    apiUrlLogout: '{ApiUtils.GetActionsLogoutUrl()}',
    apiUrlWeixin: '{ApiUtils.GetAuthUrl(OAuthType.Weixin)}?redirectUrl=' + encodeURIComponent(location.href),
    apiUrlWeibo: '{ApiUtils.GetAuthUrl(OAuthType.Weibo)}?redirectUrl=' + encodeURIComponent(location.href),
    apiUrlQq: '{ApiUtils.GetAuthUrl(OAuthType.Qq)}?redirectUrl=' + encodeURIComponent(location.href),
    registerSuccessMessage: '恭喜，注册用户成功',
    loginRedirectUrl: '?success=true',
    logoutRedirectUrl: '?logout=true',
}};
</script>
<script type=""text/javascript"" src=""{assetsUrl}/script.js""></script>
";

                CacheUtils.InsertHours(htmlPath, html, 3);
                return html;
            }
        }
    }
}
