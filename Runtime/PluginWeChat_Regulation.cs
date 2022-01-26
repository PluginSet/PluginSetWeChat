#if UNITY_EDITOR || ENABLE_WECHAT_LOGIN
using PluginSet.Core;

namespace PluginSet.WeChat
{
    public partial class PluginWeChat : IRegulationPlugin
    {
        public bool AllowPrivateTraffic => true;
        public bool ShowTosOnLoginScreen => true;
        public bool AllowGuestLogin => true;
    }
}
#endif
