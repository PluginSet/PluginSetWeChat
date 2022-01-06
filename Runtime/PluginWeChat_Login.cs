#if ENABLE_WECHAT_LOGIN
using System;
using System.Runtime.InteropServices;
using PluginSet.Core;
using PluginSet.VivoUnion;

namespace PluginSet.WeChat
{
    public partial class PluginWeChat : ILoginPlugin
    {
        public bool IsEnableLogin => sdkEnable;
        public bool IsLoggedIn { get; private set; }

        private string _loginData = null;

        private Action<Result> LocalCallback;
        
        
#if UNITY_IOS
        [DllImport("__Internal")]
        public static extern void WeChatInit(string appId,string universalLink,string u3dGameObjectName,string u3dCallName);

        [DllImport("__Internal")]
        public static extern bool IsSupportWechatLogin();

        [DllImport("__Internal")]
        public static extern void WeChatLogin();
#endif

        private void InitLogin(WeChatDelegateListener listener)
        {
            listener.OnLoginSuccess += LoginSucccess;
            listener.OnLoginFail += LoginFail;
        }

        public void Login(Action<Result> callback = null)
        {
#if UNITY_ANDROID
            WeChatAndroidApi.Jo.Call(
                "Login",
                AndroidHelper.CurrentActivity,
                new WechatCallback
                {
                    Callback = result =>
                    {
                        if (result.Success)
                        {
                            _loginData = result.Data;
                            IsLoggedIn = true;
                        }

                        callback?.Invoke(result);
                    }
                }
            );
#elif UNITY_IOS
            LocalCallback = callback;
            WeChatLogin();
#endif
        }

        /// <summary>
        /// 登陆成功
        /// </summary>
        /// <param name="result"></param>
        public void LoginSucccess(Result result)
        {
            Logger.Debug($"weixin LoginSucccess {result.Data}");
            _loginData = result.Data;
            LocalCallback?.Invoke(result);
        }

        /// <summary>
        /// 登陆失败
        /// </summary>
        /// <param name="result"></param>
        public void LoginFail(string code)
        {
            Logger.Debug($"weixin LoginFail {code}");
            LocalCallback?.Invoke(new Result
            {
                Success = false,
                Data = code,
            });
        }
        
        public void Logout(Action<Result> callback = null)
        {
            throw new NotImplementedException();
        }

        public string GetUserInfo()
        {
            return _loginData;
        }
    }
}
#endif
