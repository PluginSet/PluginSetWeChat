#if UNITY_EDITOR || ENABLE_WECHAT_PAYMENT

using System;
using PluginSet.Core;

namespace PluginSet.WeChat
{
    public partial class PluginWeChat : IPaymentPlugin
    {
#if UNITY_ANDROID
        public bool IsEnablePayment => sdkEnable;
#else
        public bool IsEnablePayment => false;
#endif

        private void InitPayment(WeChatDelegateListener listener)
        {
        }

        public void Pay(string productId, Action<Result> callback = null, string jsonData = null)
        {
#if UNITY_ANDROID
            WeChatAndroidApi.Pay(Name, productId, jsonData ?? "{}", callback);
#else
            callback?.Invoke(new Result
            {
                PluginName = Name,
                Success = false,
                Code = PluginConstants.FailDefaultCode
            });
#endif
        }
    }
}
#endif
