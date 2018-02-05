using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using SS.Login.Core;

namespace SS.Login.Api
{
    public static class ApiHttpGet
    {
        

        public static HttpResponseMessage Captcha(string code)
        {
            var ms = CacheUtils.Get($"users/captcha/{code}") as MemoryStream;

            if (ms == null)
            {
                var r = new Random();
                var randomColor = Utils.Colors[r.Next(0, 5)];

                var validateimage = new Bitmap(130, 53, PixelFormat.Format32bppRgb);

                var g = Graphics.FromImage(validateimage);
                g.FillRectangle(new SolidBrush(Color.FromArgb(240, 243, 248)), 0, 0, 200, 200); //矩形框
                g.DrawString(code, new Font(FontFamily.GenericSerif, 28, FontStyle.Bold | FontStyle.Italic), new SolidBrush(randomColor), new PointF(14, 3));//字体/颜色

                var random = new Random();

                for (var i = 0; i < 25; i++)
                {
                    var x1 = random.Next(validateimage.Width);
                    var x2 = random.Next(validateimage.Width);
                    var y1 = random.Next(validateimage.Height);
                    var y2 = random.Next(validateimage.Height);

                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }

                for (var i = 0; i < 100; i++)
                {
                    var x = random.Next(validateimage.Width);
                    var y = random.Next(validateimage.Height);

                    validateimage.SetPixel(x, y, Color.FromArgb(random.Next()));
                }

                g.Save();
                ms = new MemoryStream();
                validateimage.Save(ms, ImageFormat.Png);

                CacheUtils.InsertMinutes($"users/captcha/{code}", ms, 10);
            }

            var response = new HttpResponseMessage
            {
                Content = new ByteArrayContent(ms.ToArray())
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }
    }
}
