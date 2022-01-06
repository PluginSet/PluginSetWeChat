#if ENABLE_WECHAT_PAYMENT
using System;
using PluginSet.Core;
using PluginSet.VivoUnion;

namespace PluginSet.WeChat
{
    public partial class PluginWeChat : IPaymentPlugin
    {
        public bool IsEnablePayment => sdkEnable;

        private void InitPayment(WeChatDelegateListener listener)
        {
        }

        public void Pay(string productId, Action<Result> callback = null, string jsonData = null)
        {
#if UNITY_EDITOR
            callback?.Invoke(new Result
            {
                Success = true,
            });
#elif UNITY_ANDROID
            WeChatAndroidApi.Jo.Call(
                "Pay",
                AndroidHelper.CurrentActivity,
                jsonData,
                new WechatCallback
                {
                    Callback = result => callback?.Invoke(result)
                }
            );
#else
            throw new NotImplementedException();
#endif
        }
    }
}
#endif
