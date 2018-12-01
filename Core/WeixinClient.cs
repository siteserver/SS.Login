using System;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json.Linq;
using SiteServer.Plugin;
using SS.Login.Core.Models;

namespace SS.Login.Core
{
    //https://open.weixin.qq.com/cgi-bin/showdocument?action=dir_list&t=resource/res_list&verify=1&id=open1419316505&token=&lang=zh_CN

    public class WeixinClient
    {
        public string AppId { get; }
        public string AppSecret { get; }
        public string RedirectUrl { get; }

        public WeixinClient(string appId, string appSecret, string redirectUrl)
        {
            AppId = appId;
            AppSecret = appSecret;
            RedirectUrl = ApiUtils.GetAuthRedirectUrl(OAuthType.Weixin, redirectUrl);
        }

        public string GetAuthorizationUrl()
        {
            return
                $"https://open.weixin.qq.com/connect/qrconnect?appid={AppId}&redirect_uri={HttpUtility.UrlEncode(RedirectUrl)}&response_type=code&scope=snsapi_login&state=STATE#wechat_redirect";
        }

        private KeyValuePair<string, string> GetAccessTokenAndOpenId(string code)
        {
            var url =
                $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={AppId}&secret={AppSecret}&code={code}&grant_type=authorization_code";

            var result = Utils.HttpGet(url);

            if (result.Contains("errmsg"))
            {
                throw new Exception(result);
            }

            var data = JObject.Parse(result);

            var accessToken = data["access_token"].Value<string>();
            var openId = data["openid"].Value<string>();

            return new KeyValuePair<string, string>(accessToken, openId);
        }

        public void GetUserInfo(string code, out string nickname, out string headimgurl, out string gender, out string unionid)
        {
            nickname = headimgurl = gender = unionid = string.Empty;

            var pair = GetAccessTokenAndOpenId(code);
            var url = $"https://api.weixin.qq.com/sns/userinfo?access_token={pair.Key}&openid={pair.Value}";
            var result = Utils.HttpGet(url);

            if (result.Contains("errmsg"))
            {
                throw new Exception(result);
            }

            var data = JObject.Parse(result);
            nickname = data["nickname"].Value<string>();
            headimgurl = data["headimgurl"].Value<string>();
            gender = data["sex"].Value<string>();
            if (gender == "1")
            {
                gender = "男";
            }
            else if (gender == "2")
            {
                gender = "女";
            }
            else
            {
                gender = string.Empty;
            }
            unionid = data["unionid"].Value<string>();
        }
    }
}
