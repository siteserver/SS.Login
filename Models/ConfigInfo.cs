using System.Collections.Generic;

namespace SS.Login.Models
{
    public class ConfigInfo
    {
        public string HomeUrl { get; set; } = "/home";
        public List<string> RegisterFields { get; set; }
        public string RegisterSuccessMessage { get; set; } = "恭喜，注册用户成功";

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