using PluginSet.Core;
using PluginSet.Core.MiniJSON;
using UnityEngine;
using Logger = PluginSet.Core.Logger;

namespace PluginSet.WeChat
{
    public class WeChatDelegateListener : MonoBehaviour
    {
        private static readonly Logger Logger = LoggerManager.GetLogger("WeChat");

        public delegate void OnLoginSuccessDelegate(Result result);

        public delegate void OnLoginFailDelegate(string error);

        public delegate void OnLoginStatusChangeDelegate(string message);

        public event OnLoginSuccessDelegate OnLoginSuccess;
        public event OnLoginFailDelegate OnLoginFail;
        public event OnLoginStatusChangeDelegate OnLoginStatusChange;

        [HideInInspector]
        public string PluginName;

        public void OnPluginWeChatLoginComplete(string json)
        {
            Logger.Debug($"wechat OnPluginWeChatLoginComplete {json}");
            var dict = Json.DeserializeObject(json);
            if (dict == null)
            {
                OnLoginFail?.Invoke("Invalid Result");
                return;
            }

            //if (dict.TryGetValue("type", out var type))
            bool isSuccess = bool.Parse(dict.GetString("isSuccess"));
            if (isSuccess)
            {
                OnLoginSuccess?.Invoke(new Result
                {
                    Success = true,
                    PluginName = PluginName,
                    Data = json,
                });
                return;
            }
            else
            {
                OnLoginFail?.Invoke(json);
                return;
            }
            
            //OnAppleLoginFail("Invalid login type");
        }


        public void OnPluginAppleLoginStatusChange(string message)
        {
            OnLoginStatusChange?.Invoke(message);
        }

        private void OnDestroy()
        {
            OnLoginSuccess = null;
            OnLoginStatusChange = null;
            OnLoginFail = null;
        }
    }
}
