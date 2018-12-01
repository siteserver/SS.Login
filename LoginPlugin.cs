using System.Web;
using SiteServer.Plugin;
using SS.Login.Core;
using SS.Login.Core.Models;
using SS.Login.Core.Parse;
using SS.Login.Core.Provider;

namespace SS.Login
{
    public class LoginPlugin : PluginBase
    {
        public const string PluginId = "SS.Login";

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

        public override void Startup(IService service)
        {
            service
                .AddDatabaseTable(OAuthDao.TableName, OAuthDao.Columns)
                .AddStlElementParser(StlLogin.ElementName, StlLogin.Parse)
                .AddStlElementParser(StlLogout.ElementName, StlLogout.Parse)
                .AddStlElementParser(StlRegister.ElementName, StlRegister.Parse)
                .AddSystemMenu(() => new Menu
                {
                    Text = "第三方登录设置",
                    Href = "pages/settings.html"
                })
                ;
        }
    }
}