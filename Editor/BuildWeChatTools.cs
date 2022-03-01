using System.IO;
using System.Text;
using System.Xml;
using PluginSet.Core;
using PluginSet.Core.Editor;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace PluginSet.WeChat.Editor
{
    [BuildTools]
    public static class BuildWeChatTools
    {
        [OnSyncEditorSetting]
        public static void OnSyncEditorSetting(BuildProcessorContext context)
        {
            var buildParams = context.BuildChannels.Get<BuildWeChatParams>();
            if (!buildParams.Enable)
                return;

            context.Symbols.Add("ENABLE_WECHAT");
            if (buildParams.EnableLogin)
                context.Symbols.Add("ENABLE_WECHAT_LOGIN");

            if (buildParams.EnablePayment)
                context.Symbols.Add("ENABLE_WECHAT_PAYMENT");

            if (buildParams.EnableShare)
                context.Symbols.Add("ENABLE_WECHAT_SHARE");

            if (buildParams.EnableShareTimeline)
                context.Symbols.Add("ENABLE_WECHAT_SHARE_TIMELINE");

            context.AddLinkAssembly("PluginSet.WeChat");
            Global.CopyDependenciesFileInLib("com.pluginset.wechat", "WeChatDependencies.xml");

            var pluginConfig = context.Get<PluginSetConfig>("pluginsConfig");
            var config = pluginConfig.Get<PluginWeChatConfig>("WeChat");
            config.universalLink = buildParams.universalLink;
            config.ApiValidIfUninstalled = buildParams.ApiValidIfUninstalled;
            config.WeChatAppId = buildParams.WeChatAppId;
        }

        private static void CopyActivity(string mainPath, XmlDocument doc, string fileName)
        {
            if (string.IsNullOrEmpty(mainPath))
                throw new BuildException($"mainPath is null");

            var appId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
            var packagePath = Global.GetPackageFullPath("com.pluginset.wechat");
            var activityPath = Path.Combine(packagePath, "Editor", "JavaFiles", $"{fileName}.java");
            if (!File.Exists(activityPath))
                throw new BuildException($"{activityPath} not found");

            var targetPath = mainPath;
            foreach (var sub in appId.Split('.'))
            {
                targetPath = Path.Combine(targetPath, sub);
            }

            targetPath = Path.Combine(targetPath, "wxapi");
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            targetPath = Path.Combine(targetPath, $"{fileName}.java");

            var content = File.ReadAllLines(activityPath);
            content[0] = $"package {appId}.wxapi;";
            File.WriteAllLines(targetPath, content);

            var element = doc.FindOrAddActivity($"{appId}.wxapi.{fileName}");
            element.SetAttribute("label", AndroidConst.NS_URI, "@string/app_name");
            element.SetAttribute("theme", AndroidConst.NS_URI, "@android:style/Theme.Translucent.NoTitleBar");
            element.SetAttribute("exported", AndroidConst.NS_URI, "true");
            element.SetAttribute("taskAffinity", AndroidConst.NS_URI, $"{appId}");
            element.SetAttribute("launchMode", AndroidConst.NS_URI, "singleTask");
        }

        [AndroidProjectModify]
        public static void OnAndroidManifestModify(BuildProcessorContext context, AndroidProjectManager projectManager)
        {
            var buildParams = context.BuildChannels.Get<BuildWeChatParams>();
            if (!buildParams.Enable)
                return;

            //[1] AndroidManifest.xml 设置
            //添加必要的权限支持（其中网络权限如果没有使用扫码登录功能非必要；后三个权限，如果没有使用 mta，也非必要，即使有使用 mta，去掉也不影响功能）：
            projectManager.LibraryManifest.AddUsePermission("android.permission.INTERNET", "微信sdk");

            // 根据Android官方给出的适配方案，在主工程的AndroidManifest.xml 中增加 <queries> 标签，即可解决以上影响，代码如下：
            var doc = projectManager.LauncherManifest;
//            if (PlayerSettings.Android.targetSdkVersion.CompareTo(AndroidSdkVersions.AndroidApiLevel30) >= 0)
//            {
                const string path = "/manifest/queries/package";
                const string attrName = "name";
                const string attrValue = "com.tencent.mm";
                var list = projectManager.LibraryManifest.findElements(path, AndroidConst.NS_PREFIX, attrName, attrValue);
                if (list.Count <= 0)
                {
                    var elem = doc.createElementWithPath(path);
                    elem.SetAttribute(attrName, AndroidConst.NS_URI, attrValue);
                }
//            }

            var srcJavaPath = Path.Combine(projectManager.LibraryPath, "src", "main", "java");
            CopyActivity(srcJavaPath, doc, "WXEntryActivity");
            CopyActivity(srcJavaPath, doc, "WXPayEntryActivity");
            
            Global.AppendProguardInLib(projectManager.Proguard, "com.pluginset.wechat");
        }

        private static void ModifyIosPlistProject(BuildProcessorContext context, PlistDocument plist)
        {
            var buildParams = context.BuildChannels.Get<BuildWeChatParams>();
            if (!buildParams.Enable)
                return;

            // 添加 LSApplicationQueriesSchemes
            plist.AddApplicationQueriesSchemes("weixin");
            plist.AddApplicationQueriesSchemes("weixinULAPI");

            //添加 URL types wei xin
            plist.AddXcodeURLType("weixin", "Editor", buildParams.WeChatAppId);
        }

        [iOSXCodeProjectModify]
        public static void ModifyXCodeProject(BuildProcessorContext context, PBXProjectManager project)
        {
            var buildParams = context.BuildChannels.Get<BuildWeChatParams>();
            if (!buildParams.Enable)
                return;

            var xcodeProject = project.Project;
            var xcodeTarget = project.UnityFramework;
            xcodeProject.AddBuildProperty(xcodeTarget, "OTHER_LDFLAGS", "-all_load");

            ModifyIosPlistProject(context, project.PlistDocument);

            if (!string.IsNullOrEmpty(buildParams.universalLink))
            {
                project.AddAssociatedDomainsApplinks(buildParams.universalLink);
            }
        }
    }
}
