using PluginSet.Core.Editor;
using UnityEditor;

namespace PluginSet.WeChat.Editor
{
    [InitializeOnLoad]
    public static class WechatLibFilter
    {
        static WechatLibFilter()
        {
            var filter = PluginFilter.IsBuildParamsEnable<BuildWeChatParams>();
            PluginFilter.RegisterFilter("com.pluginset.wechat/Plugins/Android", filter);
            PluginFilter.RegisterFilter("com.pluginset.wechat/Plugins/iOS", filter);
        }
    }
}
