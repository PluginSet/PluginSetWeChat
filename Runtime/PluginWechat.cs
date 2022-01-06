#if ENABLE_WECHAT
using System.Collections;
using PluginSet.Core;
using UnityEngine;
using Logger = PluginSet.Core.Logger;

namespace PluginSet.WeChat
{
    [PluginRegister]
    public partial class PluginWeChat : PluginBase, IStartPlugin
    {
        public override string Name => "WeChat";
        
        private static readonly Logger Logger = LoggerManager.GetLogger("WeChat");
        
        public const string GameObjectName = "WeChatDelegateListener";
        
        public int StartOrder => 0;

        public bool IsRunning { get; private set; }

        private bool sdkEnable
        {
            get
            {
                if (_config.ApiValidIfUninstalled)
                    return true;

#if UNITY_EDITOR
                return true;
#elif UNITY_ANDROID
                return WeChatAndroidApi.Jo.Call<bool>("IsWeChatInstalled");
#else
                return IsSupportWechatLogin();
#endif
            }
        }

        private PluginWeChatConfig _config;

        protected override void Init(PluginSetConfig config)
        {
            _config = config.Get<PluginWeChatConfig>(Name);
        }

        public IEnumerator StartPlugin()
        {
            var obj = new GameObject(GameObjectName);
            DontDestroyOnLoad(obj);
            var listener = obj.AddComponent<WeChatDelegateListener>();
            
#if ENABLE_WECHAT_LOGIN
            InitLogin(listener);
#endif

#if ENABLE_WECHAT_PAYMENT
            InitPayment(listener);
#endif
            
#if UNITY_EDITOR
#elif UNITY_ANDROID
            WeChatAndroidApi.Jo.Call("InitApi", AndroidHelper.CurrentActivity, _config.WeChatAppId);
#elif UNITY_IOS
            WeChatInit(_config.WeChatAppId,_config.universalLink,GameObjectName,"OnPluginWeChatLoginComplete");
#else
#endif
            IsRunning = true;
            yield break;
        }

        public void DisposePlugin(bool isAppQuit = false)
        {
            IsRunning = false;
        }
    }
}
#endif
