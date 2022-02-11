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

        public void Login(Action<Result> callback = null)
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
            LocalCallback?.Invoke(result);
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
        }
        
        public void Logout(Action<Result> callback = null)
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
