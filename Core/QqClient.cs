using System;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json.Linq;
using SS.Login.Models;
using SS.Login.Parse;

namespace SS.Login.Core
{
    //http://wiki.connect.qq.com/%E5%87%86%E5%A4%87%E5%B7%A5%E4%BD%9C_oauth2-0

    public class QqClient
    {
        public string AppId { get; }
        public string AppKey { get; }
        public string RedirectUrl { get; }

        public QqClient(string appId, string appKey, string redirectUrl)
        {
            AppId = appId;
            AppKey = appKey;
            RedirectUrl = $"{LoginPlugin.Instance.PluginApi.PluginApiUrl}/{nameof(StlLogin.OAuthRedirect)}/{OAuthType.Qq.Value}?redirectUrl={HttpUtility.UrlEncode(redirectUrl)}";
        }

        public string GetAuthorizationUrl()
        {
            return
                $"https://graph.qq.com/oauth2.0/authorize?response_type=code&client_id={AppId}&redirect_uri={HttpUtility.UrlEncode(RedirectUrl)}&state=STATE";
        }

        private KeyValuePair<string, string> GetAccessTokenAndOpenId(string code)
        {
            var url =
                $"https://graph.qq.com/oauth2.0/token?grant_type=authorization_code&client_id={AppId}&client_secret={AppKey}&code={code}&redirect_uri={HttpUtility.UrlEncode(RedirectUrl)}";

            var result = Utils.HttpGet(url);
            result = result.Replace("callback(", string.Empty).Replace(");", string.Empty).Trim();

            if (result.Contains("error"))
            {
                throw new Exception(result);
            }

            var tokenData = ToDictionary(result);
            var accessToken = tokenData["access_token"];

            url = $"https://graph.qq.com/oauth2.0/me?access_token={accessToken}";

            result = Utils.HttpGet(url);
            result = result.Replace("callback(", string.Empty).Replace(");", string.Empty).Trim();

            if (result.Contains("error"))
            {
                throw new Exception(result);
            }

            var meData = JObject.Parse(result);
            var openId = meData["openid"].Value<string>();

            return new KeyValuePair<string, string>(accessToken, openId);
        }

        private static Dictionary<string, string> ToDictionary(string separateString)
        {
            var attributes = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(separateString))
            {
                var pairs = separateString.Split('&');
                foreach (var pair in pairs)
                {
                    if (pair.IndexOf("=", StringComparison.Ordinal) != -1)
                    {
                        var name = pair.Split('=')[0];
                        var value = pair.Split('=')[1];
                        attributes.Add(name, value);
                    }
                }
            }
            return attributes;
        }

        public void GetUserInfo(string code, out string displayName, out string avatarUrl, out string gender, out string uniqueId)
        {
            displayName = avatarUrl = gender = string.Empty;

            var pair = GetAccessTokenAndOpenId(code);
            uniqueId = pair.Value;

            var url = $"https://graph.qq.com/user/get_user_info?access_token={pair.Key}&oauth_consumer_key={AppId}&openid={pair.Value}";
            var result = Utils.HttpGet(url);
            result = result.Replace("callback(", string.Empty).Replace(");", string.Empty).Trim();

            var data = JObject.Parse(result);

            var ret = data["ret"].Value<int>();
            if (ret != 0)
            {
                throw new Exception(result);
            }

            displayName = data["nickname"].Value<string>();
            avatarUrl = data["figureurl_qq_1"].Value<string>();
            gender = data["gender"].Value<string>();
        }
    }
}
