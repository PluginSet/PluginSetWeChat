using System;
using PluginSet.Core;
using UnityEngine;

#if ENABLE_WECHAT
namespace PluginSet.VivoUnion
{
    public class WechatCallback : AndroidJavaProxy
    {
        public Action<Result> Callback;
        public Action<string, string> Messenger;

        public WechatCallback() : base("com.pluginset.wechatsdk.IWechatCallback")
        {
        }

        public void Call(bool success, string pluginName, string jsonData, int code, string msg)
        {
            MainThread.Run(() =>
            {
                Callback.Invoke(new Result
                {
                    Success = success,
                    PluginName = pluginName,
                    Code = code,
                    Error = msg,
                    Data = jsonData
                });
            });
        }

        public void Message(string whatMsg, string content)
        {
            Messenger?.Invoke(whatMsg, content);
        }
    }
}
#endif
