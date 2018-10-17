using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using SiteServer.Plugin;
using SS.Login.Models;

namespace SS.Login.Core
{
    public static class Utils
    {
        public static readonly Color[] Colors = { Color.FromArgb(37, 72, 91), Color.FromArgb(68, 24, 25), Color.FromArgb(17, 46, 2), Color.FromArgb(70, 16, 100), Color.FromArgb(24, 88, 74) };

        public static ConfigInfo GetConfigInfo()
        {
            return Context.ConfigApi.GetConfig<ConfigInfo>(LoginPlugin.PluginId, 0) ?? new ConfigInfo();
        }

        public static string GetMessageHtml(string message, bool isSuccess)
        {
            return isSuccess
                ? $@"<div class=""alert alert-success"" role=""alert"">{message}</div>"
                : $@"<div class=""alert alert-danger"" role=""alert"">{message}</div>";
        }

        public static List<string> GetSelectedListControlValueList(ListControl listControl)
        {
            var list = new List<string>();
            if (listControl != null)
            {
                foreach (ListItem item in listControl.Items)
                {
                    if (item.Selected)
                    {
                        list.Add(item.Value);
                    }
                }
            }
            return list;
        }

        public static void SelectListItems(ListControl listControl, params string[] values)
        {
            if (listControl != null)
            {
                foreach (ListItem item in listControl.Items)
                {
                    item.Selected = false;
                }
                foreach (ListItem item in listControl.Items)
                {
                    foreach (var value in values)
                    {
                        if (string.Equals(item.Value, value))
                        {
                            item.Selected = true;
                            break;
                        }
                    }
                }
            }
        }

        public static string ReadText(string filePath)
        {
            var sr = new StreamReader(filePath, Encoding.UTF8);
            var text = sr.ReadToEnd();
            sr.Close();
            return text;
        }

        public static string GetControlRenderHtml(Control control)
        {
            var builder = new StringBuilder();
            if (control != null)
            {
                var sw = new StringWriter(builder);
                var htw = new HtmlTextWriter(sw);
                control.RenderControl(htw);
            }
            return builder.ToString();
        }

        public static bool EqualsIgnoreCase(string a, string b)
        {
            if (a == b) return true;
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return false;
            return string.Equals(a.Trim().ToLower(), b.Trim().ToLower());
        }

        public static string HttpGet(string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36";

                //响应
                var response = (HttpWebResponse)request.GetResponse();
                var text = string.Empty;
                using (var responseStm = response.GetResponseStream())
                {
                    if (responseStm != null)
                    {
                        var redStm = new StreamReader(responseStm, Encoding.UTF8);
                        text = redStm.ReadToEnd();
                    }
                }

                return text;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
