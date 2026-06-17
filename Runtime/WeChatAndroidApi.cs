#if UNITY_EDITOR || (ENABLE_WECHAT && UNITY_ANDROID)
using System;
using PluginSet.Core;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace PluginSet.WeChat
{
    internal class WeChatLoginCallback : PluginSetCallback
    {
        public WeChatLoginCallback(string pluginName, Action<Result> callback)
            :base(delegate(string json)
            {
                if (callback == null)
                    return;
                
                MainThread.Run(() =>
                {
                    callback.Invoke(new Result
                    {
                        Success = true,
                        Code = PluginConstants.SuccessCode,
                        PluginName = pluginName,
                        Data = json
                    });
                });
                
            }, delegate(int code, string message)
            {
                if (callback == null)
                    return;
                
                MainThread.Run(() =>
                {
                    callback.Invoke(new Result
                    {
                        PluginName = pluginName,
                        Success = false,
                        Code = code,
                        Error = message
                    });
                });
            })
        {
        }
    }

    internal class WeChatPaymentCallback : PluginSetCallback
    {
        public WeChatPaymentCallback(string pluginName, string jsonData, Action<PurchaseResult> callback)
            :base(delegate(string json)
            {
                if (callback == null)
                    return;
                
                MainThread.Run(() =>
                {
                    callback?.Invoke(new PurchaseResult
                    {
                        Success = true,
                        Code = PluginConstants.SuccessCode,
                        PluginName = pluginName,
                        Data = json
                    });
                });
            }, delegate(int code, string message)
            {
                if (callback == null)
                    return;
                
                MainThread.Run(() =>
                {
                    callback?.Invoke(new PurchaseResult
                    {
                        Success = false,
                        Code = code,
                        PluginName = pluginName,
                        Error = $"Failed buy {jsonData}"
                    });
                });
            })
        {
        }
    }

    internal class WeChatShareCallback : PluginSetCallback
    {
        public WeChatShareCallback(Action success, Action fail)
            :base(delegate(string json)
            {
                MainThread.Run(success);
            }, delegate(int code, string message)
            {
                MainThread.Run(fail);
            })
        {
        }
    }

    public static class WeChatAndroidApi
    {
#if UNITY_ANDROID
        private static AndroidJavaObject _weChatApiInst;
        private static AndroidJavaObject WeChatApiInst
#else
        private static IAndroidCallable _weChatApiInst;
        private static IAndroidCallable WeChatApiInst
#endif

        {
            get
            {
#if UNITY_ANDROID
                if (_weChatApiInst == null)
                {
                    var weChatApi = new AndroidJavaClass("com.pluginset.wechat.WeChatApi");
                    _weChatApiInst = weChatApi.CallStatic<AndroidJavaObject>("GetInstance");
                }
#endif

                return _weChatApiInst;
            }
        }
        
#if UNITY_ANDROID
        private static AndroidJavaObject CurrentActivity => AndroidHelper.CurrentActivity;
#else
        private static object CurrentActivity = null;
#endif

        public static void InitApi(string appId)
        {
            WeChatApiInst?.Call("InitApi", CurrentActivity, appId);
        }

        public static bool IsWeChatInstalled()
        {
            if (WeChatApiInst == null)
                return false;
            
            return WeChatApiInst.Call<bool>("IsWeChatInstalled");
        }

        public static void Login(string pluginName, Action<Result> callback)
        {
            WeChatApiInst?.Call("Login", new WeChatLoginCallback(pluginName, callback));
        }

        public static void Pay(string pluginName, string productId, string jsonData, Action<PurchaseResult> callback)
        {
            WeChatApiInst?.Call("Pay", productId, jsonData, new WeChatPaymentCallback(pluginName, jsonData, callback));
        }

        public static void ShareText(string text, bool isTimeline, Action success, Action fail)
        {
            WeChatApiInst?.Call("ShareText", text, isTimeline, new WeChatShareCallback(success, fail));
        }

        public static void ShareImage(string path, bool isTimeline, Action success, Action fail)
        {
            WeChatApiInst?.Call("ShareImage", path, isTimeline, new WeChatShareCallback(success, fail));
        }

        public static void ShareMusicUrl(string musicUrl, bool isTimeline, Action success, Action fail, string title = null, string desc = null, string image = null)
        {
            WeChatApiInst?.Call("ShareMusic", musicUrl, isTimeline
                , new WeChatShareCallback(success, fail)
                , title, desc, image);
        }

        public static void ShareVideoUrl(string videoUrl, bool isTimeline, Action success, Action fail, string title = null, string desc = null, string image = null, string extra = null)
        {
            WeChatApiInst?.Call("ShareVideo", videoUrl, isTimeline
                , new WeChatShareCallback(success, fail)
                , title, desc, image, extra);
        }

        public static void ShareVideoFile(string filePath, bool isTimeline, Action success, Action fail, string title = null, string desc = null, string image = null, string extra = null)
        {
            WeChatApiInst?.Call("ShareVideoFile", filePath, isTimeline
                , new WeChatShareCallback(success, fail)
                , title, desc, image, extra);
        }

        public static void ShareWebPage(string url, bool isTimeline, Action success, Action fail, string title = null, string desc = null, string image = null)
        {
            WeChatApiInst?.Call("ShareWebPage", url, isTimeline
                , new WeChatShareCallback(success, fail)
                , title, desc, image);
        }

        public static void ShareMiniProgram(string url, string path, string appId, Action success, Action fail, string title = null, string desc = null, string image = null)
        {
            WeChatApiInst?.Call("ShareMiniProgram", url, path, appId
                , new WeChatShareCallback(success, fail)
                , title, desc, image);
        }


    }
}
#endif
