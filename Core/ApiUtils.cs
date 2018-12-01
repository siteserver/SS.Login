using System.Web;
using SiteServer.Plugin;
using SS.Login.Core.Models;

namespace SS.Login.Core
{
    public static class ApiUtils
    {
        public static string GetActionsLoginUrl()
        {
            return Context.UtilsApi.GetApiUrl($"{LoginPlugin.PluginId}/actions/login");
        }

        public static string GetActionsLogoutUrl()
        {
            return Context.UtilsApi.GetApiUrl($"{LoginPlugin.PluginId}/actions/logout");
        }

        public static string GetActionsRegisterUrl()
        {
            return Context.UtilsApi.GetApiUrl($"{LoginPlugin.PluginId}/actions/register");
        }

        public static string GetAuthUrl(OAuthType type)
        {
            return Context.UtilsApi.GetApiUrl($"{LoginPlugin.PluginId}/auth/{type.Value}");
        }

        public static string GetAuthUrl(OAuthType type, string redirectUrl)
        {
            return Context.UtilsApi.GetApiUrl($"{LoginPlugin.PluginId}/auth/{type.Value}?redirectUrl={HttpUtility.UrlEncode(redirectUrl)}");
        }

        public static string GetAuthRedirectUrl(OAuthType authType, string redirectUrl)
        {
            return Context.UtilsApi.GetApiUrl($"{LoginPlugin.PluginId}/auth/{authType.Value}/redirect?redirectUrl={HttpUtility.UrlEncode(redirectUrl)}");
        }

        public static string GetHomeUrl()
        {
            return Context.UtilsApi.GetHomeUrl(string.Empty);
        }
    }
}
