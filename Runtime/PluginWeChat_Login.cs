#if UNITY_EDITOR || ENABLE_WECHAT_LOGIN

using System;
using PluginSet.Core;

namespace PluginSet.WeChat
{
    public partial class PluginWeChat : ILoginPlugin
    {
        public bool IsEnableLogin => sdkEnable;
        public bool IsLoggedIn => !string.IsNullOrEmpty(_loginData);

        private string _loginData = null;

        private Action<Result> LocalCallback = null;

        private void InitLogin(WeChatDelegateListener listener)
        {
            listener.OnLoginSuccess += LoginSuccess;
            listener.OnLoginFail += LoginFail;
        }

        public void Login(Action<Result> callback = null, string _ = null)
        {
#if !UNITY_EDITOR && UNITY_IOS
            LocalCallback = callback;
            WeChatIosApi.WeChatLogin();
#else
            WeChatAndroidApi.Login(Name, delegate(Result result)
            {
                if (result.Success)
                    _loginData = result.Data;
                
                callback?.Invoke(result);
            });
#endif
        }

        /// <summary>
        /// 登陆成功
        /// </summary>
        /// <param name="result">
        /// code: token string
        /// lang: string
        /// country: string
        /// </param>
        private void LoginSuccess(Result result)
        {
            Logger.Debug($"weixin LoginSuccess {result.Data}");
            _loginData = result.Data;
            if (LocalCallback != null)
            {
                try
                {
                    LocalCallback.Invoke(result);
                }
                catch (Exception e)
                {
                    Logger.Error($"Login success callback error:{e.Message}:{e}");
                }
            }
        }

        /// <summary>
        /// 登陆失败
        /// </summary>
        /// <param name="error"></param>
        private void LoginFail(string error)
        {
            Logger.Debug($"weixin LoginFail {error}");
            LocalCallback?.Invoke(new Result
            {
                Success = false,
                PluginName = Name,
                Code = PluginConstants.FailDefaultCode,
                Error = error,
            });
            if (LocalCallback != null)
            {
                try
                {
                    LocalCallback.Invoke(new Result
                    {
                        Success = false,
                        PluginName = Name,
                        Code = PluginConstants.FailDefaultCode,
                        Error = error,
                    });
                }
                catch (Exception e)
                {
                    Logger.Error($"Login fail callback error:{e.Message}:{e}");
                }
            }
        }
        
        public void Logout(Action<Result> callback = null, string _ = null)
        {
            _loginData = null;
            
            callback?.Invoke(new  Result
            {
                PluginName = Name,
                Success = true,
                Code = PluginConstants.SuccessCode,
            });
        }

        public string GetUserLoginData()
        {
            return _loginData;
        }
    }
}
#endif
