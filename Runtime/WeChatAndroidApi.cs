using PluginSet.Core;
using UnityEngine;

#if UNITY_ANDROID
namespace PluginSet.WeChat
{
    public static class WeChatAndroidApi
    {
        private static readonly AndroidJavaClass WechatApiClass = new AndroidJavaClass("com.pluginset.wechatsdk.WeChatApi");

        private static AndroidJavaObject _api;

        public static AndroidJavaObject Jo => _api ?? (_api = WechatApiClass.CallStatic<AndroidJavaObject>("GetInstance"));
    }
}
#endif
