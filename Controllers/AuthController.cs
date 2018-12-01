using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Login.Core;
using SS.Login.Core.Models;
using SS.Login.Core.Provider;

namespace SS.Login.Controllers
{
    [RoutePrefix("auth")]
    public class AuthController : ApiController
    {
        private const string Route = "{type}";
        private const string RouteRedirect = "{type}/redirect";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetAuth(string type)
        {
            try
            {
                var request = Context.GetCurrentRequest();

                var config = Utils.GetConfigInfo();
                var oAuthType = OAuthType.Parse(type);
                var redirectUrl = request.GetQueryString("redirectUrl");
                if (string.IsNullOrEmpty(redirectUrl))
                {
                    redirectUrl = ApiUtils.GetHomeUrl();
                }

                var url = string.Empty;

                if (oAuthType == OAuthType.Weibo)
                {
                    var client = new WeiboClient(config.WeiboAppKey, config.WeiboAppSecret, redirectUrl);
                    url = client.GetAuthorizationUrl();
                }
                else if (oAuthType == OAuthType.Weixin)
                {
                    var client = new WeixinClient(config.WeixinAppId, config.WeixinAppSecret, redirectUrl);
                    url = client.GetAuthorizationUrl();
                }
                else if (oAuthType == OAuthType.Qq)
                {
                    var client = new QqClient(config.QqAppId, config.QqAppKey, redirectUrl);
                    url = client.GetAuthorizationUrl();
                }

                if (!string.IsNullOrEmpty(url))
                {
                    HttpContext.Current.Response.Redirect(url);
                }

                return BadRequest("类型不正确");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteRedirect)]
        public IHttpActionResult GetRedirect(string type)
        {
            try
            {
                var request = Context.GetCurrentRequest();

                var config = Utils.GetConfigInfo();
                var oAuthType = OAuthType.Parse(type);

                var redirectUrl = request.GetQueryString("redirectUrl");
                var code = request.GetQueryString("code");
                var userName = string.Empty;

                if (oAuthType == OAuthType.Weibo)
                {
                    var client = new WeiboClient(config.WeiboAppKey, config.WeiboAppSecret, redirectUrl);

                    string name;
                    string screenName;
                    string avatarLarge;
                    string gender;
                    string uniqueId;
                    client.GetUserInfo(code, out name, out screenName, out avatarLarge, out gender, out uniqueId);

                    userName = OAuthDao.GetUserName(OAuthType.Weibo.Value, uniqueId);
                    if (string.IsNullOrEmpty(userName))
                    {
                        var userInfo = Context.UserApi.NewInstance();
                        userInfo.UserName = Context.UserApi.IsUserNameExists(name)
                            ? Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "")
                            : name;
                        userInfo.DisplayName = screenName;
                        userInfo.AvatarUrl = avatarLarge;
                        userInfo.Gender = gender;

                        string errorMessage;
                        Context.UserApi.Insert(userInfo, Guid.NewGuid().ToString(), out errorMessage);
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
                    var client = new WeixinClient(config.WeixinAppId, config.WeixinAppSecret, redirectUrl);

                    string nickname;
                    string headimgurl;
                    string gender;
                    string unionid;
                    client.GetUserInfo(code, out nickname, out headimgurl, out gender, out unionid);

                    userName = OAuthDao.GetUserName(OAuthType.Weixin.Value, unionid);
                    if (string.IsNullOrEmpty(userName))
                    {
                        var userInfo = Context.UserApi.NewInstance();
                        userInfo.UserName = Context.UserApi.IsUserNameExists(nickname)
                            ? Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "")
                            : nickname;
                        userInfo.DisplayName = nickname;
                        userInfo.AvatarUrl = headimgurl;
                        userInfo.Gender = gender;

                        string errorMessage;
                        Context.UserApi.Insert(userInfo, Guid.NewGuid().ToString(), out errorMessage);
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
                    var client = new QqClient(config.QqAppId, config.QqAppKey, redirectUrl);

                    string displayName;
                    string avatarUrl;
                    string gender;
                    string uniqueId;
                    client.GetUserInfo(code, out displayName, out avatarUrl, out gender, out uniqueId);

                    userName = OAuthDao.GetUserName(OAuthType.Qq.Value, uniqueId);
                    if (string.IsNullOrEmpty(userName))
                    {
                        var userInfo = Context.UserApi.NewInstance();
                        userInfo.UserName = Context.UserApi.IsUserNameExists(displayName)
                            ? Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "")
                            : displayName;
                        userInfo.DisplayName = displayName;
                        userInfo.AvatarUrl = avatarUrl;
                        userInfo.Gender = gender;

                        string errorMessage;
                        Context.UserApi.Insert(userInfo, Guid.NewGuid().ToString(), out errorMessage);
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
                    request.UserLogin(userName, true);
                }

                HttpContext.Current.Response.Redirect(redirectUrl);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
