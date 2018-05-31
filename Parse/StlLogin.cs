using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.HtmlControls;
using SiteServer.Plugin;
using SS.Login.Core;
using SS.Login.Models;
using SS.Login.Provider;

namespace SS.Login.Parse
{
    public class StlLogin
    {
        private StlLogin() { }
        public const string ElementName = "stl:login";

        public const string AttributeType = "type";
        public const string AttributeRedirectUrl = "redirectUrl";

        public const string TypeAll = "all";

        public static string Parse(IParseContext context)
        {
            var type = string.Empty;
            var redirectUrl = string.Empty;

            ParseUtils.RegisterBodyHtml(context);

            var stlAnchor = new HtmlAnchor();

            foreach (var name in context.StlAttributes.AllKeys)
            {
                var value = context.StlAttributes[name];
                if (Utils.EqualsIgnoreCase(name, AttributeType))
                {
                    type = LoginPlugin.Instance.ParseApi.ParseAttributeValue(value, context);
                }
                else if (Utils.EqualsIgnoreCase(name, AttributeRedirectUrl))
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

            string text;
            var url = string.Empty;
            var onClick = string.Empty;
            if (Utils.EqualsIgnoreCase(type, OAuthType.Weibo.Value))
            {
                text = "微博登录";
                url = $"{GetOAuthApiUrl(OAuthType.Weibo)}?redirectUrl={HttpUtility.UrlEncode(redirectUrl)}";
            }
            else if (Utils.EqualsIgnoreCase(type, OAuthType.Weixin.Value))
            {
                text = "微信登录";
                url = $"{GetOAuthApiUrl(OAuthType.Weixin)}?redirectUrl={HttpUtility.UrlEncode(redirectUrl)}";
            }
            else if (Utils.EqualsIgnoreCase(type, OAuthType.Qq.Value))
            {
                text = "QQ登录";
                url = $"{GetOAuthApiUrl(OAuthType.Qq)}?redirectUrl={HttpUtility.UrlEncode(redirectUrl)}";
            }
            else if (Utils.EqualsIgnoreCase(type, TypeAll))
            {
                text = "一键登录";
                onClick = ParseUtils.OnClickLoginAll;
            }
            else
            {
                text = "登录";
                onClick = ParseUtils.OnClickLogin;
            }

            stlAnchor.HRef = string.IsNullOrEmpty(url) ? "javascript:;" : url;
            if (!string.IsNullOrEmpty(onClick))
            {
                stlAnchor.Attributes.Add("onclick", onClick);
            }

            stlAnchor.InnerHtml = LoginPlugin.Instance.ParseApi.Parse(context.StlInnerHtml, context);
            if (string.IsNullOrWhiteSpace(stlAnchor.InnerHtml))
            {
                stlAnchor.InnerHtml = text;
            }
            return Utils.GetControlRenderHtml(stlAnchor);
        }

        public static string GetApiUrlLogin()
        {
            return LoginPlugin.Instance.PluginApi.GetPluginApiUrl("actions", nameof(Login));
        }

        public static string GetOAuthApiUrl(OAuthType type)
        {
            return LoginPlugin.Instance.PluginApi.GetPluginApiUrl(nameof(OAuth), type.Value);
        }

        public static object Login(IRequest request)
        {
            var account = request.GetPostString("account");
            var password = request.GetPostString("password");

            string userName;
            string errorMessage;
            if (!LoginPlugin.Instance.UserApi.Validate(account, password, out userName, out errorMessage))
            {
                LoginPlugin.Instance.UserApi.UpdateLastActivityDateAndCountOfFailedLogin(userName);
                throw new Exception(errorMessage);
            }

            LoginPlugin.Instance.UserApi.UpdateLastActivityDateAndCountOfLogin(userName);
            var user = LoginPlugin.Instance.UserApi.GetUserInfoByUserName(userName);

            request.UserLogin(userName);

            return new
            {
                User = user
            };
        }

        public static HttpResponseMessage OAuth(IRequest context, OAuthType oAuthType)
        {
            var configInfo = Utils.GetConfigInfo();
            var redirectUrl = context.GetQueryString("redirectUrl");
            if (string.IsNullOrEmpty(redirectUrl))
            {
                redirectUrl = configInfo.HomeUrl;
            }

            var url = string.Empty;

            if (oAuthType == OAuthType.Weibo)
            {
                var client = new WeiboClient(configInfo.WeiboAppKey, configInfo.WeiboAppSecret, redirectUrl);
                url = client.GetAuthorizationUrl();
            }
            else if (oAuthType == OAuthType.Weixin)
            {
                var client = new WeixinClient(configInfo.WeixinAppId, configInfo.WeixinAppSecret, redirectUrl);
                url = client.GetAuthorizationUrl();
            }
            else if (oAuthType == OAuthType.Qq)
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

        public static HttpResponseMessage OAuthRedirect(IRequest context, OAuthType oAuthType)
        {
            var configInfo = Utils.GetConfigInfo();
            var redirectUrl = context.GetQueryString("redirectUrl");
            var code = context.GetQueryString("code");
            var userName = string.Empty;

            if (oAuthType == OAuthType.Weibo)
            {
                var client = new WeiboClient(configInfo.WeiboAppKey, configInfo.WeiboAppSecret, redirectUrl);

                string name;
                string screenName;
                string avatarLarge;
                string gender;
                string uniqueId;
                client.GetUserInfo(code, out name, out screenName, out avatarLarge, out gender, out uniqueId);

                userName = OAuthDao.GetUserName(OAuthType.Weibo.Value, uniqueId);
                if (string.IsNullOrEmpty(userName))
                {
                    var userInfo = LoginPlugin.Instance.UserApi.NewInstance();
                    userInfo.UserName = LoginPlugin.Instance.UserApi.IsUserNameExists(name) ? Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "") : name;
                    userInfo.DisplayName = screenName;
                    userInfo.AvatarUrl = avatarLarge;
                    userInfo.Gender = gender;

                    string errorMessage;
                    LoginPlugin.Instance.UserApi.Insert(userInfo, Guid.NewGuid().ToString(), out errorMessage);
                    userName = userInfo.UserName;

                    OAuthDao.Insert(new OAuthInfo
                    {
                        Source = OAuthType.Weibo.Value,
                        UniqueId = uniqueId,
                        UserName = userName
                    });
                }
            }
            else if (oAuthType == OAuthType.Weixin)
            {
                var client = new WeixinClient(configInfo.WeixinAppId, configInfo.WeixinAppSecret, redirectUrl);

                string nickname;
                string headimgurl;
                string gender;
                string unionid;
                client.GetUserInfo(code, out nickname, out headimgurl, out gender, out unionid);

                userName = OAuthDao.GetUserName(OAuthType.Weixin.Value, unionid);
                if (string.IsNullOrEmpty(userName))
                {
                    var userInfo = LoginPlugin.Instance.UserApi.NewInstance();
                    userInfo.UserName = LoginPlugin.Instance.UserApi.IsUserNameExists(nickname) ? Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "") : nickname;
                    userInfo.DisplayName = nickname;
                    userInfo.AvatarUrl = headimgurl;
                    userInfo.Gender = gender;

                    string errorMessage;
                    LoginPlugin.Instance.UserApi.Insert(userInfo, Guid.NewGuid().ToString(), out errorMessage);
                    userName = userInfo.UserName;

                    OAuthDao.Insert(new OAuthInfo
                    {
                        Source = OAuthType.Weixin.Value,
                        UniqueId = unionid,
                        UserName = userName
                    });
                }
            }
            else if (oAuthType == OAuthType.Qq)
            {
                var client = new QqClient(configInfo.QqAppId, configInfo.QqAppKey, redirectUrl);

                string displayName;
                string avatarUrl;
                string gender;
                string uniqueId;
                client.GetUserInfo(code, out displayName, out avatarUrl, out gender, out uniqueId);

                userName = OAuthDao.GetUserName(OAuthType.Qq.Value, uniqueId);
                if (string.IsNullOrEmpty(userName))
                {
                    var userInfo = LoginPlugin.Instance.UserApi.NewInstance();
                    userInfo.UserName = LoginPlugin.Instance.UserApi.IsUserNameExists(displayName) ? Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "") : displayName;
                    userInfo.DisplayName = displayName;
                    userInfo.AvatarUrl = avatarUrl;
                    userInfo.Gender = gender;

                    string errorMessage;
                    LoginPlugin.Instance.UserApi.Insert(userInfo, Guid.NewGuid().ToString(), out errorMessage);
                    userName = userInfo.UserName;

                    OAuthDao.Insert(new OAuthInfo
                    {
                        Source = OAuthType.Qq.Value,
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
    }
}
