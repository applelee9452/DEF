#if UNITY_IOS && UNITY_EDITOR

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEditor.iOS.Xcode;
    using UnityEngine;

    public static class EditorXCodePostBuild
    {
        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget build_target, string path)
        {
            if (build_target != BuildTarget.iOS) return;

            // 001.添加framework
            AddFramework(path);

            // 002.添加capability
            AddCapability(path);

            // 003.添加plist操作
            AddPlistAppKeyAndSchemes(path);
        }

        // 添加framework
        private static void AddFramework(string path)
        {
            string proj_path = PBXProject.GetPBXProjectPath(path);
            PBXProject proj = new PBXProject();
            proj.ReadFromString(File.ReadAllText(proj_path));

            // 获取当前项目名字
            string target = proj.GetUnityMainTargetGuid();//proj.TargetGuidByName(PBXProject.GetUnityTargetName());

            // 对所有的编译配置设置选项
            proj.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

            // 添加依赖库
            proj.AddFrameworkToProject(target, "libc++.dylib", false);
            proj.AddFrameworkToProject(target, "libsqlite3.dylib", false);
            proj.AddFrameworkToProject(target, "libz.dylib", false);

            proj.AddFrameworkToProject(target, "libc++.tbd", false);
            proj.AddFrameworkToProject(target, "libicucore.tbd", false);
            proj.AddFrameworkToProject(target, "libsqlite3.tbd", false);
            proj.AddFrameworkToProject(target, "libz.tbd", false);
            proj.AddFrameworkToProject(target, "libz.1.2.5.tbd", false);

            proj.AddFrameworkToProject(target, "Accelerate.framework", false);
            proj.AddFrameworkToProject(target, "AudioToolbox.framework", false);
            proj.AddFrameworkToProject(target, "AVFoundation.framework", false);
            proj.AddFrameworkToProject(target, "CFNetwork.framework", false);
            proj.AddFrameworkToProject(target, "CoreLocation.framework", false);
            proj.AddFrameworkToProject(target, "CoreMedia.framework", false);
            proj.AddFrameworkToProject(target, "CoreMotion.framework", false);
            proj.AddFrameworkToProject(target, "CoreTelephony.framework", false);
            proj.AddFrameworkToProject(target, "CoreVideo.framework", false);
            proj.AddFrameworkToProject(target, "JavaScriptCore.framework", true);// 设置为Optional
            proj.AddFrameworkToProject(target, "MessageUI.framework", false);
            proj.AddFrameworkToProject(target, "MobileCoreServices.framework", false);
            proj.AddFrameworkToProject(target, "PushKit.framework", true);
            proj.AddFrameworkToProject(target, "Security.framework", false);// 用于存储keychain
            proj.AddFrameworkToProject(target, "SystemConfiguration.framework", false);// 用于读取异常发生时的系统信息
            proj.AddFrameworkToProject(target, "UserNotifications.framework", true);
            proj.AddFrameworkToProject(target, "MediaPlayer.framework", false);
			proj.AddFrameworkToProject(target, "StoreKit.framework", false);// 内购
            proj.AddFrameworkToProject(target, "WebKit.framework", false);// UniversalLink

            // 阿里云的HttpDNS
            //proj.AddFrameworkToProject(target, "libsqlite3.0.tbd", false);
            //proj.AddFrameworkToProject(target, "libresolv.tbd", false);
            //proj.AddFrameworkToProject(target, "AlicloudHttpDNS.framework", false);
            //proj.AddFrameworkToProject(target, "UTDID.framework", false);
            //proj.AddFrameworkToProject(target, "AlicloudUtils.framework", false);
            //proj.AddFrameworkToProject(target, "UTMini.framework", false);

            // AppsFlyer
            proj.AddFrameworkToProject(target, "AdSupport.framework", false);   // 只有当您包含此框架时，AppsFlyer 才会收集 IDFA。不添加此框架就无法追踪 Facebook、Twitter 以及大多数其他广告平台
            proj.AddFrameworkToProject(target, "iAd.framework", false); // 强烈建议您将此框架添加到您的应用项目中，因为该框架是追踪 Apple Search Ads 的必备条件。

            // 本地化app应用名称添加

            //var infoDirs = Directory.GetDirectories (Application.dataPath + "/iOS/Localization/infoPlist/");
            //for (var i = 0; i < infoDirs.Length; ++i) {
            //    var files = Directory.GetFiles (infoDirs [i], "*.strings");
            //    proj.AddLocalization(files [0], "InfoPlist.strings", "InfoPlists.strings");
            //}
 
            //var localdirs = Directory.GetDirectories (Application.dataPath + "/iOS/Localization/infoPlist/");
            //for (var i = 0; i < localdirs.Length; ++i) {
            //    var files = Directory.GetFiles (localdirs [i], "*.strings");
            //    proj.AddLocalization (files [0], "Localizable.strings", "Localizable.strings");
            //}

            // 设置签名
            //proj.SetBuildProperty (target, "CODE_SIGN_IDENTITY", "iPhone Distribution: _______________");
            //proj.SetBuildProperty (target, "PROVISIONING_PROFILE", "********-****-****-****-************");
            //string debugConfig = proj.BuildConfigByName(target, "Debug");
            //string releaseConfig = proj.BuildConfigByName(target, "Release");

            //proj.SetBuildPropertyForConfig(debugConfig, "PROVISIONING_PROFILE", "证书");
            //proj.SetBuildPropertyForConfig(releaseConfig, "PROVISIONING_PROFILE", "证书");
            //proj.SetBuildPropertyForConfig(debugConfig, "PROVISIONING_PROFILE(Deprecated)", "证书");
            //proj.SetBuildPropertyForConfig(releaseConfig, "PROVISIONING_PROFILE(Deprecated)", "证书");

            //proj.SetBuildPropertyForConfig(debugConfig, "CODE_SIGN_IDENTITY", "自己的证书");
            //proj.SetBuildPropertyForConfig(releaseConfig, "CODE_SIGN_IDENTITY", "自己的证书");

            //proj.SetTeamId(target, "Team ID(自己的teamid)");

            //proj.AddCapability(target, PBXCapabilityType.AssociatedDomains);

            // 保存工程
            proj.WriteToFile(proj_path);
        }

        // 修改capability 授权
        private static void AddCapability(string path)
        {
            string proj_path = PBXProject.GetPBXProjectPath(path);
            PBXProject proj = new PBXProject();
            proj.ReadFromString(File.ReadAllText(proj_path));

            ProjectCapabilityManager proj_capability_mgr = new ProjectCapabilityManager(proj_path, "xxx.entitlements", proj.GetUnityMainTargetGuid());
            proj_capability_mgr.AddAssociatedDomains(new string[] { "applinks:xx.xxx.xxx" });
            proj_capability_mgr.AddPushNotifications(false);
            proj_capability_mgr.AddBackgroundModes(BackgroundModesOptions.RemoteNotifications);
			proj_capability_mgr.AddInAppPurchase();// Add the In-App Purchase capability
            proj_capability_mgr.WriteToFile();
        }

        // plist修改
        private static void AddPlistAppKeyAndSchemes(string path)
        {
            // 修改plist
            string plist_path = path + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plist_path));
            PlistElementDict root_dict = plist.root;
            root_dict.SetString("com.openinstall.APP_KEY", "znc4d4");// OpenInstall

            // NativeFun
            root_dict.SetString("NSPhotoLibraryUsageDescription", "Requires access to the Photo Library");
            root_dict.SetString("NSPhotoLibraryAddUsageDescription", "Requires access to the Photo Library");
            root_dict.SetString("NSCameraUsageDescription", "Requires access to the Camera");
            root_dict.SetString("NSContactsUsageDescription", "Requires access to Contacts");
            //root_dict.SetString("NSLocationAlwaysUsageDescription", "Requires access to Location");
            //root_dict.SetString("NSLocationWhenInUseUsageDescription", "Requires access to Location");
            //root_dict.SetString("NSLocationAlwaysAndWhenInUseUsageDescription", "Requires access to Location");

            // OpenInstall scheme
            PlistElementArray urlArray = null;
            if (!root_dict.values.ContainsKey("CFBundleURLTypes"))
            {
                urlArray = root_dict.CreateArray("CFBundleURLTypes");
            }
            else
            {
                urlArray = root_dict.values["CFBundleURLTypes"].AsArray();
            }
            var urlTypeDict = urlArray.AddDict();
            urlTypeDict.SetString("CFBundleURLName", "OpenInstall");
            var urlScheme = urlTypeDict.CreateArray("CFBundleURLSchemes");
            urlScheme.AddString("znc4d4");
			
			var urlTypeDictOrange = urlArray.AddDict();
            urlTypeDictOrange.SetString("CFBundleURLName", "weixin");
            var urlScheme_weixin = urlTypeDictOrange.CreateArray("CFBundleURLSchemes");
            urlScheme_weixin.AddString("wxff929d92c3997b5d");
			
			//LSApplicationQueriesSchemes
			PlistElementArray app_schemes_array = null;
			if (!root_dict.values.ContainsKey("LSApplicationQueriesSchemes"))
            {
                app_schemes_array = root_dict.CreateArray("LSApplicationQueriesSchemes");
            }
            else
            {
                app_schemes_array = root_dict.values["LSApplicationQueriesSchemes"].AsArray();
            }
			app_schemes_array.AddString("weixin");
			app_schemes_array.AddString("weixinULAPI");

            // 保存plist
            plist.WriteToFile(plist_path);
        }
    }

#endif
