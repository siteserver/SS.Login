namespace SS.Login.Core.Models
{
    public class ConfigInfo
    {
        public bool IsWeibo { get; set; } = false;
        public string WeiboAppKey { get; set; }
        public string WeiboAppSecret { get; set; }

        public bool IsWeixin { get; set; } = false;
        public string WeixinAppId { get; set; }
        public string WeixinAppSecret { get; set; }

        public bool IsQq { get; set; } = false;
        public string QqAppId { get; set; }
        public string QqAppKey { get; set; }
    }
}