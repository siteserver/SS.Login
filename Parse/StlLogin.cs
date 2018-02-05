using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.HtmlControls;
using SiteServer.Plugin;
using SS.Login.Core;
using SS.Login.Model;
using SS.Login.Provider;

namespace SS.Login.Parse
{
    public class StlLogin
    {
        private StlLogin() { }
        public const string ElementName = "stl:login";

        public const string AttributeType = "type";
        public const string AttributeRedirectUrl = "redirectUrl";

        public const string TypeWeibo = "weibo";
        public const string TypeWeixin = "weixin";
        public const string TypeQq = "qq";
        public const string TypeAll = "all";

        public static string GetApiUrlLogin()
        {
            return Main.Instance.PluginApi.GetPluginApiUrl("actions", nameof(Login));
        }

        public static string GetApiUrlWeibo()
        {
            return Main.Instance.PluginApi.GetPluginApiUrl(nameof(OAuth), TypeWeibo);
        }

        public static string GetApiUrlWeixin()
        {
            return Main.Instance.PluginApi.GetPluginApiUrl(nameof(OAuth), TypeWeixin);
        }

        public static string GetApiUrlQq()
        {
            return Main.Instance.PluginApi.GetPluginApiUrl(nameof(OAuth), TypeQq);
        }

        public static object Login(IRequest request)
        {
            var account = request.GetPostString("account");
            var password = request.GetPostString("password");

            string userName;
            string errorMessage;
            if (!Main.Instance.UserApi.Validate(account, password, out userName, out errorMessage))
            {
                Main.Instance.UserApi.UpdateLastActivityDateAndCountOfFailedLogin(userName);
                throw new Exception(errorMessage);
            }

            Main.Instance.UserApi.UpdateLastActivityDateAndCountOfLogin(userName);
            var user = Main.Instance.UserApi.GetUserInfoByUserName(userName);

            request.UserLogin(userName);

            return new
            {
                User = user
            };
        }

        public static HttpResponseMessage OAuth(IRequest context, string loginType)
        {
            var configInfo = Utils.GetConfigInfo();
            var redirectUrl = context.GetQueryString("redirectUrl");
            if (string.IsNullOrEmpty(redirectUrl))
            {
                redirectUrl = configInfo.HomeUrl;
            }

            var url = string.Empty;

            if (Utils.EqualsIgnoreCase(loginType, TypeWeibo))
            {
                var client = new WeiboClient(configInfo.WeiboAppKey, configInfo.WeiboAppSecret, redirectUrl);
                url = client.GetAuthorizationUrl();
            }
            else if (Utils.EqualsIgnoreCase(loginType, TypeWeixin))
            {
                var client = new WeixinClient(configInfo.WeixinAppId, configInfo.WeixinAppSecret, redirectUrl);
                url = client.GetAuthorizationUrl();
            }
            else if (Utils.EqualsIgnoreCase(loginType, TypeQq))
            {
                var client = new QqClient(configInfo.QqAppId, configInfo.QqAppKey, redirectUrl);
                url = client.GetAuthorizationUrl();
            }

            if (!string.IsNullOrEmpty(url))
            {
                var response = new HttpResponseMessage
                {
                    Content = new StringContent($"<script>location.href = '{url}';</script>"),
                    StatusCode = HttpStatusCode.OK
                };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

                return response;
            }

            throw new Exception("类型不正确");
        }

        public static HttpResponseMessage OAuthRedirect(IRequest context, string loginType)
        {
            var configInfo = Utils.GetConfigInfo();
            var redirectUrl = context.GetQueryString("redirectUrl");
            var code = context.GetQueryString("code");
            var userName = string.Empty;

            if (Utils.EqualsIgnoreCase(loginType, TypeWeibo))
            {
                var client = new WeiboClient(configInfo.WeiboAppKey, configInfo.WeiboAppSecret, redirectUrl);

                string name;
                string screenName;
                string avatarLarge;
                string gender;
                string uniqueId;
                client.GetUserInfo(code, out name, out screenName, out avatarLarge, out gender, out uniqueId);

                userName = OAuthDao.GetUserName(TypeWeibo, uniqueId);
                if (string.IsNullOrEmpty(userName))
                {
                    var userInfo = Main.Instance.UserApi.NewInstance();
                    userInfo.UserName = Main.Instance.UserApi.IsUserNameExists(name) ? Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "") : name;
                    userInfo.DisplayName = screenName;
                    userInfo.AvatarUrl = avatarLarge;
                    userInfo.Gender = gender;

                    string errorMessage;
                    Main.Instance.UserApi.Insert(userInfo, Guid.NewGuid().ToString(), out errorMessage);
                    userName = userInfo.UserName;

                    OAuthDao.Insert(new OAuthInfo
                    {
                        Source = TypeWeibo,
                        UniqueId = uniqueId,
                        UserName = userName
                    });
                }
            }
            else if (Utils.EqualsIgnoreCase(loginType, TypeWeixin))
            {
                var client = new WeixinClient(configInfo.WeixinAppId, configInfo.WeixinAppSecret, redirectUrl);

                string nickname;
                string headimgurl;
                string gender;
                string unionid;
                client.GetUserInfo(code, out nickname, out headimgurl, out gender, out unionid);

                userName = OAuthDao.GetUserName(TypeWeixin, unionid);
                if (string.IsNullOrEmpty(userName))
                {
                    var userInfo = Main.Instance.UserApi.NewInstance();
                    userInfo.UserName = Main.Instance.UserApi.IsUserNameExists(nickname) ? Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "") : nickname;
                    userInfo.DisplayName = nickname;
                    userInfo.AvatarUrl = headimgurl;
                    userInfo.Gender = gender;

                    string errorMessage;
                    Main.Instance.UserApi.Insert(userInfo, Guid.NewGuid().ToString(), out errorMessage);
                    userName = userInfo.UserName;

                    OAuthDao.Insert(new OAuthInfo
                    {
                        Source = TypeWeixin,
                        UniqueId = unionid,
                        UserName = userName
                    });
                }
            }
            else if (Utils.EqualsIgnoreCase(loginType, TypeQq))
            {
                var client = new QqClient(configInfo.QqAppId, configInfo.QqAppKey, redirectUrl);

                string displayName;
                string avatarUrl;
                string gender;
                string uniqueId;
                client.GetUserInfo(code, out displayName, out avatarUrl, out gender, out uniqueId);

                userName = OAuthDao.GetUserName(TypeQq, uniqueId);
                if (string.IsNullOrEmpty(userName))
                {
                    var userInfo = Main.Instance.UserApi.NewInstance();
                    userInfo.UserName = Main.Instance.UserApi.IsUserNameExists(displayName) ? Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "") : displayName;
                    userInfo.DisplayName = displayName;
                    userInfo.AvatarUrl = avatarUrl;
                    userInfo.Gender = gender;

                    string errorMessage;
                    Main.Instance.UserApi.Insert(userInfo, Guid.NewGuid().ToString(), out errorMessage);
                    userName = userInfo.UserName;

                    OAuthDao.Insert(new OAuthInfo
                    {
                        Source = TypeQq,
                        UniqueId = uniqueId,
                        UserName = userName
                    });
                }
            }

            if (!string.IsNullOrEmpty(userName))
            {
                context.UserLogin(userName);
            }

            var response = new HttpResponseMessage
            {
                Content = new StringContent($"<script>location.href = '{redirectUrl}';</script>"),
                StatusCode = HttpStatusCode.OK
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }

        public static string Parse(IParseContext context)
        {
            var type = string.Empty;
            var redirectUrl = string.Empty;

            if (!context.BodyCodes.ContainsKey(ParseUtils.GlobalHtmlCodeKey))
            {
                context.BodyCodes.Add(ParseUtils.GlobalHtmlCodeKey, ParseUtils.GetGlobalHtml());
            }

            var stlAnchor = new HtmlAnchor();

            foreach (var name in context.StlElementAttributes.Keys)
            {
                var value = context.StlElementAttributes[name];
                if (Utils.EqualsIgnoreCase(name, AttributeType))
                {
                    type = Main.Instance.ParseApi.ParseAttributeValue(value, context);
                }
                else if (Utils.EqualsIgnoreCase(name, AttributeRedirectUrl))
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


            var url = string.Empty;
            var onClick = string.Empty;
            if (Utils.EqualsIgnoreCase(type, TypeWeibo))
            {
                url = $"{GetApiUrlWeibo()}?redirectUrl={HttpUtility.UrlEncode(redirectUrl)}";
            }
            else if (Utils.EqualsIgnoreCase(type, TypeWeixin))
            {
                url = $"{GetApiUrlWeixin()}?redirectUrl={HttpUtility.UrlEncode(redirectUrl)}";
            }
            else if (Utils.EqualsIgnoreCase(type, TypeQq))
            {
                url = $"{GetApiUrlQq()}?redirectUrl={HttpUtility.UrlEncode(redirectUrl)}";
            }
            else if (Utils.EqualsIgnoreCase(type, TypeAll))
            {
                onClick = ParseUtils.OnClickLoginAll;
            }
            else
            {
                onClick = ParseUtils.OnClickLogin;
            }

            stlAnchor.HRef = string.IsNullOrEmpty(url) ? "javascript:;" : url;
            if (!string.IsNullOrEmpty(onClick))
            {
                stlAnchor.Attributes.Add("onclick", onClick);
            }

            stlAnchor.InnerHtml = Main.Instance.ParseApi.ParseInnerXml(context.StlElementInnerXml, context);
            return Utils.GetControlRenderHtml(stlAnchor);
        }
    }
}
