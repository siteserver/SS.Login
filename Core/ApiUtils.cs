using System.Web;
using SiteServer.Plugin;

namespace SS.Login.Core
{
    public static class ApiUtils
    {
        public static string GetActionsLoginUrl()
        {
            return $"{Context.Environment.ApiUrl}/{Plugin.PluginId}/actions/login";
        }

        public static string GetActionsLogoutUrl()
        {
            return $"{Context.Environment.ApiUrl}/{Plugin.PluginId}/actions/logout";
        }

        public static string GetActionsRegisterUrl()
        {
            return $"{Context.Environment.ApiUrl}/{Plugin.PluginId}/actions/register";
        }

        public static string GetAuthUrl(OAuthType type)
        {
            return $"{Context.Environment.ApiUrl}/{Plugin.PluginId}/auth/{type.Value}";
        }

        public static string GetAuthUrl(OAuthType type, string redirectUrl)
        {
            return $"{Context.Environment.ApiUrl}/{Plugin.PluginId}/auth/{type.Value}?redirectUrl={HttpUtility.UrlEncode(redirectUrl)}";
        }

        public static string GetAuthRedirectUrl(OAuthType authType, string redirectUrl)
        {
            return $"{Context.Environment.ApiUrl}/{Plugin.PluginId}/auth/{authType.Value}/redirect?redirectUrl={HttpUtility.UrlEncode(redirectUrl)}";
        }

        public static string GetHomeUrl()
        {
            return Context.UtilsApi.GetHomeUrl(string.Empty);
        }
    }
}
