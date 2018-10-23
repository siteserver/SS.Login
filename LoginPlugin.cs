using System;
using System.Collections.Generic;
using System.Web;
using SiteServer.Plugin;
using SS.Login.Api;
using SS.Login.Core;
using SS.Login.Models;
using SS.Login.Pages;
using SS.Login.Parse;
using SS.Login.Provider;

namespace SS.Login
{
    public class LoginPlugin : PluginBase
    {
        public static string PluginId { get; private set; }

        public static IConfigApi ConfigApi => Context.ConfigApi;
        public static IRequest Request => Context.Request;

        public static bool IsOAuthReady(OAuthType oAuthType)
        {
            var configInfo = Utils.GetConfigInfo();
            if (oAuthType == OAuthType.Weibo)
            {
                return configInfo.IsWeibo;
            }
            if (oAuthType == OAuthType.Weixin)
            {
                return configInfo.IsWeixin;
            }
            if (oAuthType == OAuthType.Qq)
            {
                return configInfo.IsQq;
            }
            return false;
        }

        public static string GetOAuthLoginUrl(OAuthType oAuthType, string redirectUrl)
        {
            return $"{StlLogin.GetOAuthApiUrl(oAuthType)}?redirectUrl={HttpUtility.UrlEncode(redirectUrl)}";
        }

        public override void Startup(IService service)
        {
            PluginId = Id;

            service
                .AddDatabaseTable(OAuthDao.TableName, OAuthDao.Columns)
                .AddStlElementParser(StlLogin.ElementName, StlLogin.Parse)
                .AddStlElementParser(StlLogout.ElementName, StlLogout.Parse)
                .AddStlElementParser(StlRegister.ElementName, StlRegister.Parse)
                .AddSystemMenu(() => new Menu
                {
                    Text = "用户登录设置",
                    Menus = new List<Menu>
                    {
                        new Menu
                        {
                            Text = "授权认证设置",
                            Href = $"SS.Login.Files/{nameof(PageSettings)}.aspx"
                        },
                        new Menu
                        {
                            Text = "第三方登录设置",
                            Href = $"SS.Login.Files/{nameof(PageOAuth)}.aspx"
                        }
                    }
                })
                ;

            service.RestApiGet += Service_RestApiGet;
            service.RestApiPost += Service_RestApiPost;
        }

        private object Service_RestApiGet(object sender, RestApiEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.RouteResource) && !string.IsNullOrEmpty(args.RouteId))
            {
                if (Utils.EqualsIgnoreCase(args.RouteResource, nameof(StlLogin.OAuth)))
                {
                    return StlLogin.OAuth(args.Request, OAuthType.Parse(args.RouteId));
                }
                if (Utils.EqualsIgnoreCase(args.RouteResource, nameof(StlLogin.OAuthRedirect)))
                {
                    return StlLogin.OAuthRedirect(args.Request, OAuthType.Parse(args.RouteId));
                }
                if (Utils.EqualsIgnoreCase(args.RouteResource, nameof(ApiHttpGet.Captcha)))
                {
                    return ApiHttpGet.Captcha(args.RouteId);
                }
            }

            throw new Exception("请求的资源不在服务器上");
        }

        private static object Service_RestApiPost(object sender, RestApiEventArgs args)
        {
            var request = args.Request;

            if (Utils.EqualsIgnoreCase(args.RouteAction, nameof(StlRegister.Register)))
            {
                return StlRegister.Register(request);
            }
            if (Utils.EqualsIgnoreCase(args.RouteAction, nameof(StlLogin.Login)))
            {
                return StlLogin.Login(request);
            }
            if (Utils.EqualsIgnoreCase(args.RouteAction, nameof(StlLogout.Logout)))
            {
                return StlLogout.Logout(request);
            }

            if (Utils.EqualsIgnoreCase(args.RouteAction, nameof(ApiJsonActionsPost.ResetPassword)))
            {
                return ApiJsonActionsPost.ResetPassword(request);
            }
            if (Utils.EqualsIgnoreCase(args.RouteAction, nameof(ApiJsonActionsPost.ResetPasswordByToken)))
            {
                return ApiJsonActionsPost.ResetPasswordByToken(request);
            }
            if (Utils.EqualsIgnoreCase(args.RouteAction, nameof(ApiJsonActionsPost.Edit)))
            {
                return ApiJsonActionsPost.Edit(request);
            }
            if (Utils.EqualsIgnoreCase(args.RouteAction, nameof(ApiJsonActionsPost.IsMobileExists)))
            {
                return ApiJsonActionsPost.IsMobileExists(request);
            }
            if (Utils.EqualsIgnoreCase(args.RouteAction, nameof(ApiJsonActionsPost.IsPasswordCorrect)))
            {
                return ApiJsonActionsPost.IsPasswordCorrect(request);
            }
            if (Utils.EqualsIgnoreCase(args.RouteAction, nameof(ApiJsonActionsPost.IsCodeCorrect)))
            {
                return ApiJsonActionsPost.IsCodeCorrect(request);
            }

            throw new Exception("请求的资源不在服务器上");
        }
    }
}