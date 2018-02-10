using System;
using SS.Login.Core;
using SS.Login.Models;

namespace SS.Login.Parse
{
    public static class ParseUtils
    {
        public const string OnClickLogin = "$authVue.openLoginModal()";
        public const string OnClickLoginAll = "$authVue.openSocialModal()";
        public const string OnClickRegister = "$authVue.openRegisterModal()";
        public const string OnClickLogout = "$authVue.logout()";

        public static string GlobalHtmlCodeKey = "";

        public static string GetGlobalHtml()
        {
            var htmlPath = LoginPlugin.Instance.PluginApi.GetPluginPath("assets/template.html");
            var assetsUrl = LoginPlugin.Instance.PluginApi.GetPluginUrl("assets");
            var html = CacheUtils.Get<string>(htmlPath);
            if (html == null)
            {
                html = Utils.ReadText(htmlPath);
                var startIndex = html.IndexOf("<!-- template start -->", StringComparison.Ordinal);
                var length = html.IndexOf("<!-- template end -->", StringComparison.Ordinal) - startIndex;
                html = html.Substring(startIndex, length);
                html = html.Replace(@"<script type=""text/javascript"" src=""",
                    $@"<script type=""text/javascript"" src=""{assetsUrl}/");
                html = html.Replace(@"<link rel=""stylesheet"" type=""text/css"" href=""",
                    $@"<link rel=""stylesheet"" type=""text/css"" href=""{assetsUrl}/");
                CacheUtils.InsertHours(htmlPath, html, 3);
            }

            html += $@"
<script type=""text/javascript"">
authData = {{
    apiUrlRegister: '{StlRegister.GetApiUrlRegister()}',
    apiUrlLogin: '{StlLogin.GetApiUrlLogin()}',
    apiUrlLogout: '{StlLogout.GetApiUrlLogout()}',
    apiUrlWeixin: '{StlLogin.GetOAuthApiUrl(OAuthType.Weixin)}?redirectUrl=' + location.href,
    apiUrlWeibo: '{StlLogin.GetOAuthApiUrl(OAuthType.Weibo)}?redirectUrl=' + location.href,
    apiUrlQq: '{StlLogin.GetOAuthApiUrl(OAuthType.Qq)}?redirectUrl=' + location.href,
    registerSuccessMessage: '恭喜，注册用户成功',
    loginRedirectUrl: '?success=true',
    logoutRedirectUrl: '?logout=true',
}};
</script>";

            return html;
        }
    }
}
