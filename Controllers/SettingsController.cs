using System;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Login.Core;
using SS.Login.Core.Models;

namespace SS.Login.Controllers
{
    [RoutePrefix("settings")]
    public class SettingsController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = Context.GetCurrentRequest();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSystemPermissions(LoginPlugin.PluginId)) return Unauthorized();

                var config = Utils.GetConfigInfo();
                var urlWeibo = string.Empty;
                var urlWeixin = string.Empty;
                var urlQq = string.Empty;

                if (config.IsWeibo)
                {
                    urlWeibo = ApiUtils.GetAuthUrl(OAuthType.Weibo, ApiUtils.GetHomeUrl());
                }

                if (config.IsWeixin)
                {
                    urlWeixin = ApiUtils.GetAuthUrl(OAuthType.Weixin, ApiUtils.GetHomeUrl());
                }
                
                if (config.IsQq)
                {
                    urlQq = ApiUtils.GetAuthUrl(OAuthType.Qq, ApiUtils.GetHomeUrl());
                }

                return Ok(new
                {
                    Value = config,
                    UrlWeibo = urlWeibo,
                    UrlWeixin = urlWeixin,
                    UrlQq = urlQq
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var request = Context.GetCurrentRequest();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSystemPermissions(LoginPlugin.PluginId)) return Unauthorized();

                var config = Utils.GetConfigInfo();

                if (request.IsPostExists("isWeibo"))
                {
                    config.IsWeibo = request.GetPostBool("isWeibo");
                }
                if (request.IsPostExists("weiboAppKey"))
                {
                    config.WeiboAppKey = request.GetPostString("weiboAppKey");
                }
                if (request.IsPostExists("weiboAppSecret"))
                {
                    config.WeiboAppSecret = request.GetPostString("weiboAppSecret");
                }

                if (request.IsPostExists("isWeixin"))
                {
                    config.IsWeixin = request.GetPostBool("isWeixin");
                }
                if (request.IsPostExists("weixinAppId"))
                {
                    config.WeixinAppId = request.GetPostString("weixinAppId");
                }
                if (request.IsPostExists("weixinAppSecret"))
                {
                    config.WeixinAppSecret = request.GetPostString("weixinAppSecret");
                }

                if (request.IsPostExists("isQq"))
                {
                    config.IsQq = request.GetPostBool("isQq");
                }
                if (request.IsPostExists("qqAppId"))
                {
                    config.QqAppId = request.GetPostString("qqAppId");
                }
                if (request.IsPostExists("qqAppKey"))
                {
                    config.QqAppKey = request.GetPostString("qqAppKey");
                }

                Context.ConfigApi.SetConfig(LoginPlugin.PluginId, 0, config);

                var urlWeibo = string.Empty;
                var urlWeixin = string.Empty;
                var urlQq = string.Empty;

                if (config.IsWeibo)
                {
                    urlWeibo = ApiUtils.GetAuthUrl(OAuthType.Weibo, ApiUtils.GetHomeUrl());
                }

                if (config.IsWeixin)
                {
                    urlWeixin = ApiUtils.GetAuthUrl(OAuthType.Weixin, ApiUtils.GetHomeUrl());
                }

                if (config.IsQq)
                {
                    urlQq = ApiUtils.GetAuthUrl(OAuthType.Qq, ApiUtils.GetHomeUrl());
                }

                return Ok(new
                {
                    Value = config,
                    UrlWeibo = urlWeibo,
                    UrlWeixin = urlWeixin,
                    UrlQq = urlQq
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
