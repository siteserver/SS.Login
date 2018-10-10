using System;
using SiteServer.Plugin;
using SS.Login.Core;

namespace SS.Login.Api
{
    public static class ApiJsonActionsPost
    {
        private static string GetSendSmsCacheKey(string mobile)
        {
            return $"SS.Home.Api.ActionsPost.SendSms.{mobile}.Code";
        }

        public static object ResetPassword(IRequest request)
        {
            if (!request.IsUserLoggin)
            {
                throw new Exception("用户未登录");
            }

            var password = request.GetPostString("password");
            var newPassword = request.GetPostString("newPassword");
            var confirmPassword = request.GetPostString("confirmPassword");

            string userName;
            string errorMessage;
            if (newPassword != confirmPassword)
            {
                throw new Exception("确认密码与新密码不一致，请重新输入");
            }
            if (string.IsNullOrEmpty(password) || !LoginPlugin.Instance.UserApi.Validate(request.UserName, password, out userName, out errorMessage))
            {
                throw new Exception("原密码输入错误，请重新输入");
            }
            if (password == newPassword)
            {
                throw new Exception("新密码不能与原密码一致，请重新输入");
            }

            if (LoginPlugin.Instance.UserApi.ChangePassword(request.UserName, newPassword, out errorMessage))
            {
                return new
                {
                    LastResetPasswordDate = DateTime.Now
                };
            }

            throw new Exception(errorMessage);
        }

        public static object ResetPasswordByToken(IRequest request)
        {
            var token = request.GetPostString("token");
            var password = request.GetPostString("password");

            var accessToken = LoginPlugin.Instance.UserApi.ParseAccessToken(token);
            if (accessToken == null || accessToken.UserId == 0 || string.IsNullOrEmpty(accessToken.UserName))
            {
                throw new Exception("用户认证失败");
            }

            string errorMessage;
            var isSuccess = LoginPlugin.Instance.UserApi.ChangePassword(accessToken.UserName, password, out errorMessage);

            return new
            {
                IsSuccess = isSuccess,
                ErrorMessage = errorMessage
            };
        }

        public static object Edit(IRequest request)
        {
            if (!request.IsUserLoggin)
            {
                throw new Exception("用户认证失败");
            }

            var userInfo = LoginPlugin.Instance.UserApi.GetUserInfoByUserId(request.UserId);
            
            if (userInfo == null)
            {
                throw new Exception("用户认证失败");
            }

            if (request.GetPostString("avatarUrl") != null)
            {
                userInfo.AvatarUrl = request.GetPostString("avatarUrl");
            }
            if (request.GetPostString("displayName") != null)
            {
                userInfo.DisplayName = request.GetPostString("displayName");
            }
            if (request.GetPostString("gender") != null)
            {
                userInfo.Gender = request.GetPostString("gender");
            }
            if (request.GetPostString("birthday") != null)
            {
                userInfo.Birthday = request.GetPostString("birthday");
            }
            if (request.GetPostString("mobile") != null)
            {
                var mobile = request.GetPostString("mobile");
                if (mobile != userInfo.Mobile)
                {
                    var exists = LoginPlugin.Instance.UserApi.IsMobileExists(mobile);
                    if (!exists)
                    {
                        userInfo.Mobile = mobile;
                        LoginPlugin.Instance.UserApi.AddLog(userInfo.UserName, "修改手机", string.Empty);
                    }
                    else
                    {
                        throw new Exception("此手机号码已注册，请更换手机号码");
                    }
                }
            }
            if (request.GetPostString("email") != null)
            {
                var email = request.GetPostString("email");
                if (email != userInfo.Email)
                {
                    var exists = LoginPlugin.Instance.UserApi.IsEmailExists(email);
                    if (!exists)
                    {
                        userInfo.Email = email;
                        LoginPlugin.Instance.UserApi.AddLog(userInfo.UserName, "修改邮箱", string.Empty);
                    }
                    else
                    {
                        throw new Exception("此邮箱已注册，请更换邮箱");
                    }
                }
            }

            LoginPlugin.Instance.UserApi.Update(userInfo);
            return userInfo;
        }

        public static object IsMobileExists(IRequest request)
        {
            var mobile = request.GetPostString("mobile");
            return new
            {
                Exists = LoginPlugin.Instance.UserApi.IsMobileExists(mobile)
            };
        }

        public static object IsPasswordCorrect(IRequest request)
        {
            var password = request.GetPostString("password");
            string errorMessage;
            var isCorrect = LoginPlugin.Instance.UserApi.IsPasswordCorrect(password, out errorMessage);
            return new
            {
                IsCorrect = isCorrect,
                ErrorMessage = errorMessage
            };
        }

        public static object IsCodeCorrect(IRequest request)
        {
            var mobile = request.GetPostString("mobile");
            var code = request.GetPostString("code");

            var dbCode = CacheUtils.Get<string>(GetSendSmsCacheKey(mobile));

            var isCorrect = code == dbCode;
            var token = string.Empty;
            if (isCorrect)
            {
                var userInfo = LoginPlugin.Instance.UserApi.GetUserInfoByMobile(mobile);
                if (userInfo != null)
                {
                    token = LoginPlugin.Instance.UserApi.GetAccessToken(userInfo.Id, userInfo.UserName, DateTime.Now.AddDays(7));
                }
            }

            return new
            {
                IsCorrect = isCorrect,
                Token = token
            };
        }
    }
}
