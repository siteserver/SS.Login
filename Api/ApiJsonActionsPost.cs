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
            if (string.IsNullOrEmpty(password) || !Main.Instance.UserApi.Validate(request.UserName, password, out userName, out errorMessage))
            {
                throw new Exception("原密码输入错误，请重新输入");
            }
            if (password == newPassword)
            {
                throw new Exception("新密码不能与原密码一致，请重新输入");
            }

            if (Main.Instance.UserApi.ChangePassword(request.UserName, newPassword, out errorMessage))
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

            var userName = request.GetUserNameByToken(token);
            if (string.IsNullOrEmpty(userName))
            {
                throw new Exception("用户认证失败");
            }

            string errorMessage;
            var isSuccess = Main.Instance.UserApi.ChangePassword(userName, password, out errorMessage);

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

            var userInfo = request.UserInfo;
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
            if (request.GetPostString("signature") != null)
            {
                userInfo.Signature = request.GetPostString("signature");
            }
            if (request.GetPostString("organization") != null)
            {
                userInfo.Organization = request.GetPostString("organization");
            }
            if (request.GetPostString("department") != null)
            {
                userInfo.Department = request.GetPostString("department");
            }
            if (request.GetPostString("position") != null)
            {
                userInfo.Position = request.GetPostString("position");
            }
            if (request.GetPostString("education") != null)
            {
                userInfo.Education = request.GetPostString("education");
            }
            if (request.GetPostString("graduation") != null)
            {
                userInfo.Graduation = request.GetPostString("graduation");
            }
            if (request.GetPostString("address") != null)
            {
                userInfo.Address = request.GetPostString("address");
            }
            if (request.GetPostString("interests") != null)
            {
                userInfo.Interests = request.GetPostString("interests");
            }
            if (request.GetPostString("mobile") != null)
            {
                var mobile = request.GetPostString("mobile");
                if (mobile != userInfo.Mobile)
                {
                    var exists = Main.Instance.UserApi.IsMobileExists(mobile);
                    if (!exists)
                    {
                        userInfo.Mobile = mobile;
                        Main.Instance.UserApi.AddLog(userInfo.UserName, "修改手机", string.Empty);
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
                    var exists = Main.Instance.UserApi.IsEmailExists(email);
                    if (!exists)
                    {
                        userInfo.Email = email;
                        Main.Instance.UserApi.AddLog(userInfo.UserName, "修改邮箱", string.Empty);
                    }
                    else
                    {
                        throw new Exception("此邮箱已注册，请更换邮箱");
                    }
                }
            }

            Main.Instance.UserApi.Update(userInfo);
            return userInfo;
        }

        public static object IsMobileExists(IRequest request)
        {
            var mobile = request.GetPostString("mobile");
            return new
            {
                Exists = Main.Instance.UserApi.IsMobileExists(mobile)
            };
        }

        public static object IsPasswordCorrect(IRequest request)
        {
            var password = request.GetPostString("password");
            string errorMessage;
            var isCorrect = Main.Instance.UserApi.IsPasswordCorrect(password, out errorMessage);
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
                var userInfo = Main.Instance.UserApi.GetUserInfoByMobile(mobile);
                if (userInfo != null)
                {
                    token = request.GetUserTokenByUserName(userInfo.UserName);
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
