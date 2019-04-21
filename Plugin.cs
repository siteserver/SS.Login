using SiteServer.Plugin;
using SS.Login.Core;

namespace SS.Login
{
    public class Plugin : PluginBase
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

        public static OAuthRepository OAuthRepository { get; set; }

        public override void Startup(IService service)
        {
            OAuthRepository = new OAuthRepository();

            service
                .AddDatabaseTable(OAuthRepository.TableName, OAuthRepository.TableColumns)
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