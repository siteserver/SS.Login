namespace SS.Login.Core
{
    public class ConfigInfo
    {
        public bool IsWeibo { get; set; }
        public string WeiboAppKey { get; set; }
        public string WeiboAppSecret { get; set; }

        public bool IsWeixin { get; set; }
        public string WeixinAppId { get; set; }
        public string WeixinAppSecret { get; set; }

        public bool IsQq { get; set; }
        public string QqAppId { get; set; }
        public string QqAppKey { get; set; }
    }
}