using PluginSet.Core;
using UnityEngine;

namespace PluginSet.WeChat
{
    public class PluginWeChatConfig: ScriptableObject
    {
        public bool ApiValidIfUninstalled;
        public string WeChatAppId;
        public string universalLink;
    }
}