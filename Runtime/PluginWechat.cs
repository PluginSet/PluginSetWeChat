#if UNITY_EDITOR || ENABLE_WECHAT
using System.Collections;
using PluginSet.Core;
using UnityEngine;
using Logger = PluginSet.Core.Logger;

namespace PluginSet.WeChat
{
#if ENABLE_WECHAT
    [PluginRegister]
#endif
    public partial class PluginWeChat : PluginBase, IStartPlugin
    {
        public override string Name => "WeChat";
        
        private static readonly Logger Logger = LoggerManager.GetLogger("WeChat");
        
        private const string GameObjectName = "WeChatDelegateListener";
        
        public int StartOrder => PluginsStartOrder.SdkDefault;

        public bool IsRunning { get; private set; }

        private bool sdkEnable
        {
            get
            {
                if (_config.ApiValidIfUninstalled)
                    return true;

#if !UNITY_EDITOR && UNITY_IOS
                return WeChatIosApi.IsSupportWeChatLogin();
#else
                return WeChatAndroidApi.IsWeChatInstalled();
#endif
            }
        }

        private PluginWeChatConfig _config;

        protected override void Init(PluginSetConfig config)
        {
            _config = config.Get<PluginWeChatConfig>();
        }

        public IEnumerator StartPlugin()
        {
            var obj = new GameObject(GameObjectName);
            DontDestroyOnLoad(obj);
            var listener = obj.AddComponent<WeChatDelegateListener>();
            listener.PluginName = Name;
#if UNITY_EDITOR || ENABLE_WECHAT_LOGIN
            InitLogin(listener);
#endif
#if UNITY_EDITOR || ENABLE_WECHAT_PAYMENT
            InitPayment(listener);
#endif
            
#if !UNITY_EDITOR && UNITY_IOS
            WeChatIosApi.WeChatInit(_config.WeChatAppId,_config.universalLink,GameObjectName,"OnPluginWeChatLoginComplete");
#else
            WeChatAndroidApi.InitApi(_config.WeChatAppId);
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
