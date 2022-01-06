using PluginSet.Core;
using PluginSet.Core.Editor;
using UnityEngine;

namespace PluginSet.WeChat.Editor
{
    [BuildChannelsParams("WeChat", "微信SDK参数")]
    public class BuildWeChatParams : ScriptableObject
    {
        [Tooltip("是否启动微信SDK")]
        public bool Enable;

        [Tooltip("是否启动微信SDK登录")]
        [VisibleCaseBoolValue("Enable", true)]
        public bool EnableLogin;

        [Tooltip("是否启动微信SDK支付")]
        [VisibleCaseBoolValue("Enable", true)]
        public bool EnablePayment;

        [Tooltip("是否启动微信SDK分享")]
        [VisibleCaseBoolValue("Enable", true)]
        public bool EnableShare;

        [Tooltip("是否启动微信TimeLine分享")]
        [VisibleCaseBoolValue("Enable", true)]
        public bool EnableShareTimeline = true;

        [Tooltip("微信未安装时API是否可用")]
        public bool ApiValidIfUninstalled;

        [Tooltip("微信SDK提供的AppId参数")]
        public string WeChatAppId;

        public string universalLink;
    }
}
