using PluginSet.Core;
using UnityEngine;

namespace PluginSet.WeChat
{
    [PluginSetConfig("WeChat")]
    public class PluginWeChatConfig: ScriptableObject
    {
        public bool ApiValidIfUninstalled;
        public string WeChatAppId;
        public string universalLink;
    }
}