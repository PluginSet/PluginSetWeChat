#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;

namespace PluginSet.WeChat
{
    public static class WeChatIosApi
    {
        [DllImport("__Internal")]
        public static extern void WeChatInit(string appId,string universalLink,string u3dGameObjectName,string u3dCallName);

        [DllImport("__Internal")]
        public static extern bool IsSupportWeChatLogin();

        [DllImport("__Internal")]
        public static extern void WeChatLogin();
    }
}
#endif
