using PluginSet.Core.Editor;
using UnityEditor;

namespace PluginSet.WeChat.Editor
{
    [InitializeOnLoad]
    public static class WechatLibFilter
    {
        static WechatLibFilter()
        {
            PluginFilter.RegisterFilterExpress<BuildWeChatParams>("com.pluginset.wechat", "Plugins/Android");
            PluginFilter.RegisterFilterExpress<BuildWeChatParams>("com.pluginset.wechat", "Plugins/iOS");
        }
    }
}
