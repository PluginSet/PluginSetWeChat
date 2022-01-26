#if UNITY_EDITOR || ENABLE_WECHAT_SHARE_TIMELINE
using System;
using PluginSet.Core;

namespace PluginSet.WeChat
{
#if !UNITY_EDITOR
    [PluginRegister]
#endif
    public class PluginWeChatTimeline: PluginBase, IShareTextPlugin, IShareImagePlugin, IShareMusicUrlPlugin
    , IShareVideoPlugin, IShareVideoUrlPlugin, IShareWebPagePlugin
    {
        public override string Name => "WeChatTimeline";

        public bool IsEnableShare
        {
            get
            {
                if (_apiValidIfUninstalled)
                    return true;
                
#if UNITY_ANDROID
                return WeChatAndroidApi.IsWeChatInstalled();
#else
                return false;
#endif
            }
        }

        private const bool IS_TIMELINE = true;
        
        private bool _apiValidIfUninstalled;
        
        protected override void Init(PluginSetConfig config)
        {
            var weChat = config.Get<PluginWeChatConfig>("WeChat");
            _apiValidIfUninstalled = weChat.ApiValidIfUninstalled;
        }

        public void ShareText(string text, Action success = null, Action fail = null
            , string title = null, string image = null)
        {
#if UNITY_ANDROID
            WeChatAndroidApi.ShareText(text, IS_TIMELINE, success, fail);
#else
            throw new NotImplementedException();
#endif
        }
        
        public void ShareImage(string imagePath, Action success = null, Action fail = null
            , string title = null, string desc = null, string image = null)
        {
#if UNITY_ANDROID
            WeChatAndroidApi.ShareImage(imagePath, IS_TIMELINE, success, fail);
#else
            throw new NotImplementedException();
#endif
        }

        public void ShareMusicUrl(string musicUrl, Action success = null, Action fail = null
            , string title = null, string desc = null, string image = null)
        {
#if UNITY_ANDROID
            WeChatAndroidApi.ShareMusicUrl(musicUrl, IS_TIMELINE, success, fail, title, desc, image);
#else
            throw new NotImplementedException();
#endif
        }

        public void ShareVideo(string videoFile, Action success = null, Action fail = null, string extra = null
            , string title = null, string desc = null, string image = null)
        {
#if UNITY_ANDROID
            WeChatAndroidApi.ShareVideoFile(videoFile, IS_TIMELINE, success, fail, title, desc, image, extra);
#else
            throw new NotImplementedException();
#endif
        }

        public void ShareVideoUrl(string videoUrl, Action success = null, Action fail = null, string extra = null
            , string title = null, string desc = null, string image = null)
        {
#if UNITY_ANDROID
            WeChatAndroidApi.ShareVideoUrl(videoUrl, IS_TIMELINE, success, fail, title, desc, image, extra);
#else
            throw new NotImplementedException();
#endif
        }
        
        public void ShareWebPage(string webUrl, Action success = null, Action fail = null
            , string title = null, string desc = null, string image = null)
        {
#if UNITY_ANDROID
            WeChatAndroidApi.ShareWebPage(webUrl, IS_TIMELINE, success, fail, title, desc, image);
#else
            throw new NotImplementedException();
#endif
        }

    }
}
#endif