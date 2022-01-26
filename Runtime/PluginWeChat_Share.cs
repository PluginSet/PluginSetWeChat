#if UNITY_EDITOR || ENABLE_WECHAT_SHARE
using System;
using PluginSet.Core;

namespace PluginSet.WeChat
{
    public partial class PluginWeChat:
        IShareTextPlugin, IShareImagePlugin, IShareMusicUrlPlugin, IShareVideoPlugin
        , IShareVideoUrlPlugin, IShareWebPagePlugin, IShareMiniProgramPlugin
    {
        private const bool IS_TIMELINE = false;
        
        public bool IsEnableShare => sdkEnable;
        
        public void ShareText(string text, Action success = null, Action fail = null
            , string title = null, string image = null)
        {
#if UNITY_ANDROID
            WeChatAndroidApi.ShareText(text, IS_TIMELINE, success, fail);
#else
            fail?.Invoke();
#endif
        }
        
        public void ShareImage(string imagePath, Action success = null, Action fail = null
            , string title = null, string desc = null, string image = null)
        {
#if UNITY_ANDROID
            WeChatAndroidApi.ShareImage(imagePath, IS_TIMELINE, success, fail);
#else
            fail?.Invoke();
#endif
        }

        public void ShareMusicUrl(string musicUrl, Action success = null, Action fail = null
            , string title = null, string desc = null, string image = null)
        {
#if UNITY_ANDROID
            WeChatAndroidApi.ShareMusicUrl(musicUrl, IS_TIMELINE, success, fail, title, desc, image);
#else
            fail?.Invoke();
#endif
        }

        public void ShareVideo(string videoFile, Action success = null, Action fail = null, string extra = null
            , string title = null, string desc = null, string image = null)
        {
#if UNITY_ANDROID
            WeChatAndroidApi.ShareVideoFile(videoFile, IS_TIMELINE, success, fail, title, desc, image, extra);
#else
            fail?.Invoke();
#endif
        }

        public void ShareVideoUrl(string videoUrl, Action success = null, Action fail = null, string extra = null
            , string title = null, string desc = null, string image = null)
        {
#if UNITY_ANDROID
            WeChatAndroidApi.ShareVideoUrl(videoUrl, IS_TIMELINE, success, fail, title, desc, image, extra);
#else
            fail?.Invoke();
#endif
        }
        
        public void ShareWebPage(string webUrl, Action success = null, Action fail = null
            , string title = null, string desc = null, string image = null)
        {
#if UNITY_ANDROID
            WeChatAndroidApi.ShareWebPage(webUrl, IS_TIMELINE, success, fail, title, desc, image);
#else
            fail?.Invoke();
#endif
        }

        public void ShareMiniProgram(string url, string path, string appId, Action success = null, Action fail = null
            , string title = null, string desc = null, string image = null)
        {
#if UNITY_ANDROID
            WeChatAndroidApi.ShareMiniProgram(url, path,appId, success, fail, title, desc, image);
#else
            fail?.Invoke();
#endif
        }
    }
}
#endif