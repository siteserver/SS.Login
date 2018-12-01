using System;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Login.Core;

namespace SS.Login.Controllers
{
    [RoutePrefix("actions")]
    public class ActionsController : ApiController
    {
        private const string RouteRegister = "register";
        private const string RouteLogin = "login";
        private const string RouteLogout = "logout";
        private const string RouteResetPassword = "resetpassword";
        private const string RouteResetPasswordByToken = "resetpasswordbytoken";
        private const string RouteEdit = "edit";
        private const string RouteIsMobileExists = "ismobileexists";
        private const string RouteIsPasswordCorrect = "ispasswordcorrect";
        private const string RouteIsCodeCorrect = "iscodecorrect";

        [HttpPost, Route(RouteRegister)]
        public IHttpActionResult Register()
        {
            try
            {
                var request = Context.GetCurrentRequest();

                var userName = request.GetPostString("userName");
                var displayName = request.GetPostString("displayName");
                var email = request.GetPostString("email");
                var mobile = request.GetPostString("mobile");
                var password = request.GetPostString("password");

                var userInfo = Context.UserApi.NewInstance();
                userInfo.UserName = userName;
                userInfo.DisplayName = displayName;
                userInfo.Email = email;
                userInfo.Mobile = mobile;

                string errorMessage;
                if (!Context.UserApi.Insert(userInfo, password, out errorMessage))
                {
                    return BadRequest(errorMessage);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteLogin)]
        public IHttpActionResult Login()
        {
            try
            {
                var request = Context.GetCurrentRequest();

                var account = request.GetPostString("account");
                var password = request.GetPostString("password");

                IUserInfo userInfo;
                string userName;
                string errorMessage;
                if (!Context.UserApi.Validate(account, password, out userName, out errorMessage))
                {
                    userInfo = Context.UserApi.GetUserInfoByUserName(userName);
                    if (userInfo != null)
                    {
                        userInfo.CountOfFailedLogin += 1;
                        userInfo.LastActivityDate = DateTime.Now;
                        Context.UserApi.Update(userInfo);
                    }
                    return BadRequest(errorMessage);
                }

                userInfo = Context.UserApi.GetUserInfoByUserName(userName);
                userInfo.CountOfFailedLogin = 0;
                userInfo.CountOfLogin += 1;
                userInfo.LastActivityDate = DateTime.Now;
                Context.UserApi.Update(userInfo);

                request.UserLogin(userName, true);

                return Ok(new
                {
                    Value = userInfo
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteLogout)]
        public IHttpActionResult Logout()
        {
            try
            {
                var request = Context.GetCurrentRequest();

                request.UserLogout();

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteResetPassword)]
        public IHttpActionResult ResetPassword()
        {
            try
            {
                var request = Context.GetCurrentRequest();

                if (!request.IsUserLoggin)
                {
                    return BadRequest("用户未登录");
                }

                var password = request.GetPostString("password");
                var newPassword = request.GetPostString("newPassword");
                var confirmPassword = request.GetPostString("confirmPassword");

                string userName;
                string errorMessage;
                if (newPassword != confirmPassword)
                {
                    return BadRequest("确认密码与新密码不一致，请重新输入");
                }
                if (string.IsNullOrEmpty(password) || !Context.UserApi.Validate(request.UserName, password, out userName, out errorMessage))
                {
                    return BadRequest("原密码输入错误，请重新输入");
                }
                if (password == newPassword)
                {
                    return BadRequest("新密码不能与原密码一致，请重新输入");
                }

                if (!Context.UserApi.ChangePassword(request.UserName, newPassword, out errorMessage))
                {
                    return BadRequest(errorMessage);
                }

                return Ok(new
                {
                    Value = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteResetPasswordByToken)]
        public IHttpActionResult ResetPasswordByToken()
        {
            try
            {
                var request = Context.GetCurrentRequest();

                var token = request.GetPostString("token");
                var password = request.GetPostString("password");

                var accessToken = Context.UserApi.ParseAccessToken(token);
                if (accessToken == null || accessToken.UserId == 0 || string.IsNullOrEmpty(accessToken.UserName))
                {
                    return BadRequest("用户认证失败");
                }

                string errorMessage;
                var isSuccess = Context.UserApi.ChangePassword(accessToken.UserName, password, out errorMessage);

                return Ok(new
                {
                    Value = isSuccess,
                    ErrorMessage = errorMessage
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        
        [HttpPost, Route(RouteEdit)]
        public IHttpActionResult Edit()
        {
            try
            {
                var request = Context.GetCurrentRequest();

                if (!request.IsUserLoggin)
                {
                    return BadRequest("用户认证失败");
                }

                var userInfo = Context.UserApi.GetUserInfoByUserId(request.UserId);

                if (userInfo == null)
                {
                    return BadRequest("用户认证失败");
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
                        var exists = Context.UserApi.IsMobileExists(mobile);
                        if (!exists)
                        {
                            userInfo.Mobile = mobile;
                            Context.UserApi.AddLog(userInfo.UserName, "修改手机", string.Empty);
                        }
                        else
                        {
                            return BadRequest("此手机号码已注册，请更换手机号码");
                        }
                    }
                }
                if (request.GetPostString("email") != null)
                {
                    var email = request.GetPostString("email");
                    if (email != userInfo.Email)
                    {
                        var exists = Context.UserApi.IsEmailExists(email);
                        if (!exists)
                        {
                            userInfo.Email = email;
                            Context.UserApi.AddLog(userInfo.UserName, "修改邮箱", string.Empty);
                        }
                        else
                        {
                            return BadRequest("此邮箱已注册，请更换邮箱");
                        }
                    }
                }

                Context.UserApi.Update(userInfo);

                return Ok(new
                {
                    Value = userInfo
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteIsMobileExists)]
        public IHttpActionResult IsMobileExists()
        {
            try
            {
                var request = Context.GetCurrentRequest();

                var mobile = request.GetPostString("mobile");

                return Ok(new
                {
                    Value = Context.UserApi.IsMobileExists(mobile)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteIsPasswordCorrect)]
        public IHttpActionResult IsPasswordCorrect()
        {
            try
            {
                var request = Context.GetCurrentRequest();

                var password = request.GetPostString("password");
                string errorMessage;
                var isCorrect = Context.UserApi.IsPasswordCorrect(password, out errorMessage);

                return Ok(new
                {
                    Value = isCorrect,
                    ErrorMessage = errorMessage
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private static string GetSendSmsCacheKey(string mobile)
        {
            return $"SS.Home.Api.ActionsPost.SendSms.{mobile}.Code";
        }

        [HttpPost, Route(RouteIsCodeCorrect)]
        public IHttpActionResult IsCodeCorrect()
        {
            try
            {
                var request = Context.GetCurrentRequest();

                var mobile = request.GetPostString("mobile");
                var code = request.GetPostString("code");

                var dbCode = CacheUtils.Get<string>(GetSendSmsCacheKey(mobile));

                var isCorrect = code == dbCode;
                var token = string.Empty;
                if (isCorrect)
                {
                    var userInfo = Context.UserApi.GetUserInfoByMobile(mobile);
                    if (userInfo != null)
                    {
                        token = Context.UserApi.GetAccessToken(userInfo.Id, userInfo.UserName, DateTime.Now.AddDays(7));
                    }
                }

                return Ok(new
                {
                    Value = isCorrect,
                    Token = token
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
